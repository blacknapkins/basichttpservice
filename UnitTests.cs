using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhoneBookApp.Controllers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using PhoneBookApp.Models;
using System.Collections;
using System.Net;

namespace UnitTestProject1
{
    /// <summary>
    ///     Test PhoneBookController class
    /// </summary>
    /// <remarks>
    ///     <para/>@Author      Peter Anderson
    ///     <para/>@Version     1.0.0.0
    ///     <para/>@Date        2015-09-21; Peter Anderson; Initial Version
    ///     <para/>@Copyright   Copyright 2015.
    /// </remarks>

    [TestClass]
    public class UnitTests : ApiController
    {
        Entry Entry1 = new Entry { Id = 1, Firstname = "Tony", Surname = "Benn", Address = "Home", PhoneNo = "999" };
        Entry Entry2 = new Entry { Id = 2, Firstname = "Jeremy", Surname = "Corbyn", PhoneNo = "888" };
        Entry Entry3 = new Entry { Id = 3, Firstname = "David", Surname = "Cameron", Address = "Away", PhoneNo = "777" };
        Entry Entry4 = new Entry { Id = 3, Firstname = "Diane", Surname = "Abbott", Address = "Away", PhoneNo = "777" };
        Entry Entry5 = new Entry { Id = 5, Firstname = "Tony", Surname = "Benn", Address = "Home", PhoneNo = "999" };
        Entry Entry6 = new Entry { Id = 6, Firstname = "Hilary", Surname = "Benn", PhoneNo = "888" };
        Entry Entry7 = new Entry { Id = 7, Firstname = "David", Surname = "Cameron", Address = "Away", PhoneNo = "777" };
        Entry Entry8 = new Entry { Id = 8, Firstname = "Diane", Surname = "Abbott", Address = "Rodeo Drive", PhoneNo = "666" }; 
        Entry invalidEntry = new Entry { Id = 98, Surname = "Corbyn", PhoneNo = "888" };

        PhoneBookController thisPBController = new PhoneBookController();


        [TestMethod]
        // Test add entry using POST
        // Check we have increased the number of entries by one
        // Requirement "Create a new entry to the phone book"
        public void TestMethod1()
        {
            IHttpActionResult FaultStatus;

            FaultStatus = thisPBController.Post(Entry1);
            var response = FaultStatus as CreatedAtRouteNegotiatedContentResult<Entry>;
            Assert.IsNotNull(response);
            Assert.AreEqual("DefaultApi", response.RouteName);
            Assert.AreEqual(response.Content, Entry1);
            Assert.AreEqual(1, response.RouteValues["id"]);
            Assert.AreEqual(1, thisPBController.PhoneBookControllerGetNoOfEntries());

            FaultStatus = thisPBController.GetEntry(1);
            var response2 = FaultStatus as OkNegotiatedContentResult<Entry>;
            Assert.IsNotNull(response2);
            Assert.AreEqual(response2.Content, Entry1);

            // Clean up
            thisPBController.Delete(1);
        }

        [TestMethod]
        // Test not finding entry
        public void TestMethod2()
        {
            IHttpActionResult FaultStatus;

            thisPBController.Post(Entry2);
            FaultStatus = thisPBController.GetEntry(1);
            var response = FaultStatus as OkNegotiatedContentResult<Entry>;
            Assert.IsNull(response);

            // Clean up
            thisPBController.Delete(2);
        }

        [TestMethod]

        // Test deleting entry using DELETE
        // Requirement "Remove an existing entry in the phonebook"
        public void TestMethod3()
        {
            IHttpActionResult FaultStatus;
            IHttpActionResult response;

            thisPBController.Post(Entry1);
            FaultStatus = thisPBController.Delete(1);
            Assert.IsInstanceOfType(FaultStatus, typeof(OkResult));

            FaultStatus = thisPBController.GetEntry(1);
            response = FaultStatus as OkNegotiatedContentResult<Entry>;
            Assert.IsNull(response);
        }

        [TestMethod]
        // Test add 3 entries and checking we get all of them
        // Requirement "List all entries in the phonebook"
        public void TestMethod4()
        {
            IEnumerable entries;
            IEnumerator EntryEnumerator;
            int count = 0;

            thisPBController.Post(Entry1);
            thisPBController.Post(Entry2);
            thisPBController.Post(Entry3);
            entries = thisPBController.GetAllEntries();
            EntryEnumerator = entries.GetEnumerator();
            {
                while (EntryEnumerator.MoveNext())
                    count++;
            }
            Assert.AreEqual(3, count);

            // Clean up
            thisPBController.Delete(1);
            thisPBController.Delete(2);
            thisPBController.Delete(3);
        }

        [TestMethod]
        // Test update an existing entry using PUT
        // Check we have the same number of elements after calling
        // Requirement "Update an existing entry in the phonebook"
        public void TestMethod5()
        {
            IHttpActionResult FaultStatus;
            int noOfElementsBefore = 0;
            int noOfElementsAfter = 0;

            // Add David Cameron
            FaultStatus = thisPBController.Post(Entry3);
            noOfElementsBefore = thisPBController.PhoneBookControllerGetNoOfEntries();
            // Replace with Diane Abbott
            FaultStatus = thisPBController.Put(3, Entry4);
            var contentResult = FaultStatus as NegotiatedContentResult<Entry>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(3, contentResult.Content.Id);
            // Check David Cameron has been replaced by Diane Abbott
            Assert.AreEqual("Diane", contentResult.Content.Firstname);
            Assert.AreEqual("Abbott", contentResult.Content.Surname);

            // Check put has not increased number of elements
            noOfElementsAfter = thisPBController.PhoneBookControllerGetNoOfEntries();
            Assert.AreEqual(noOfElementsBefore, noOfElementsAfter);


            // Clean up
            thisPBController.Delete(2);
        }

        [TestMethod]
        // Test add 2 entries with same surname and checking we get all of them
        // Requirement "Search for entries in the phone book by surname"
        public void TestMethod6()
        {
            IEnumerable foundEntries;
            IEnumerator EntryEnumerator;
            int count = 0;

            thisPBController.Post(Entry5);
            thisPBController.Post(Entry6);
            thisPBController.Post(Entry7);
            thisPBController.Post(Entry8);
            foundEntries = thisPBController.GetAllMatchedEntries("Benn");
            Assert.IsNotNull(foundEntries);
            EntryEnumerator = foundEntries.GetEnumerator();
            {
                while (EntryEnumerator.MoveNext())
                    count++;
            }
            // Should find Tony and Hilary Benn
            Assert.AreEqual(2, count);

            // Clean up
            thisPBController.Delete(5);
            thisPBController.Delete(6);
            thisPBController.Delete(7);
            thisPBController.Delete(8);
        }

        [TestMethod]
        // Test adding invalid entry using POST
        // Check we have not increased the number of entries by one
        public void TestMethod7()
        {
            IHttpActionResult FaultStatus;

            FaultStatus = thisPBController.Post(invalidEntry);
            var response = FaultStatus as CreatedAtRouteNegotiatedContentResult<Entry>;
            Assert.IsNotNull(response);
            Assert.AreEqual("", response.RouteName);
            // Invalid entry should return invalid Id
            Assert.AreEqual(Entry.INVALID_ID, response.RouteValues["id"]);
            // Not updated number of entries
            Assert.AreEqual(0, thisPBController.PhoneBookControllerGetNoOfEntries());

            // Clean up
            thisPBController.Delete(1);

        }
    }
}
