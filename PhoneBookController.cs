using PhoneBookApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net;


namespace PhoneBookApp.Controllers
{
    /// <summary>
    ///     PhoneBookController handles http requests    
    /// </summary>
    /// <remarks>
    ///     <para/>@Author      Peter Anderson
    ///     <para/>@Version     1.0.0.0
    ///     <para/>@Date        2015-09-21; Peter Anderson; Initial Version
    ///     <para/>@Copyright   Copyright 2015.
    /// </remarks>
    public class PhoneBookController : ApiController
    {
        // Pick a simple list in a real system this would be some sort of
        // persistent media
        List<Entry> phoneBook;

        public PhoneBookController()
        {
            phoneBook = new List<Entry>();
        }

        /// <summary>
        /// Determine whether an Entry is valid or not.
        /// </summary>
        /// <param name="thisEntry">The entry to validate </param>
        /// <returns> 
        /// true if valid otherwise false
        /// </returns>
        /// <remarks>
        /// Id may also have to be range checked
        /// </remarks>
        ///
        private bool isEntryValid(Entry thisEntry)
        {
            if ((thisEntry.Firstname == null) || 
                (thisEntry.Surname == null) || 
                (thisEntry.PhoneNo == null) || 
                (thisEntry.Id == Entry.INVALID_ID))
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Initialises phonebook storage.
        ///In a more complicated system this may do something like set up a 
        /// database connection or start loading a cache. 
        /// </summary>
        /// <returns> 
        /// Whether initialisation has completed successfully or appropriate
        /// error code
        /// </returns>
        ///
        public int PhoneBookControllerInit()
        {
            // Just stub this out for now
            return 1;
        }

        /// <summary>
        /// Gets total number of entries.
        /// </summary>
        /// <returns> 
        /// No of entries in the phonebook
        /// </returns>
        ///
        public int PhoneBookControllerGetNoOfEntries()
        {
            return phoneBook.Count();
        }

        /// <summary>
        /// Performs an ordered shutdown
        /// In a more complicated system this may do something like flush
        /// a cache or free resources
        /// </summary>
        ///
        public void PhoneBookControllerShutdown()
        {
            // Just stub this out for now
        }


        /// <summary>
        /// A software reset is required but not a shutdown
        /// In a more complicated system this may do something like reload the
        /// cache
        /// </summary>
        /// <returns>
        /// Whether the warm start  has completed successfully or appropriate
        /// error code
        /// </returns>
        ///
        public int PhoneBookControllerWarmStart()
        {
            //Just stub this out for now
            return 1;
        }


        /// <summary>
        /// Responds to GETALL request.
        /// List all entries requirement
        /// </summary>
        /// <returns>
        /// A list of all phonebook entries
        /// </returns>
        ///
        public IEnumerable<Entry> GetAllEntries()
        {
            return phoneBook;
        }


        /// <summary>
        /// Responds to GET request with surname
        /// List all entries with a given surname
        /// </summary>
        /// <param name="Surname">The surname property to search for </param>
        /// <returns>
        /// A list of all entries matched to the Surname property
        /// </returns>
        ///
        public IEnumerable<Entry> GetAllMatchedEntries(String Surname)
        {
            List<Entry> foundList = phoneBook.FindAll(s => s.Surname == Surname);
            return foundList;
        }

        /// <summary>
        /// Respond to GET request with ID.
        /// </summary>
        /// <param name="id">The Entry to get from the phonebook </param>
        ///<returns>
        /// Result of GetEntry request and Entry data if available
        ///</returns> 
        ///
        public IHttpActionResult GetEntry(int id)
        {
            Entry FoundEntry;

            FoundEntry = phoneBook.FirstOrDefault((p) => p.Id == id);
            if (FoundEntry == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(FoundEntry);
            }
        }

        /// <summary>
        /// Respond to POST request.
        /// Create new entry requirement.
        /// </summary>
        /// <param name="newEntry">The entry to add to the phonebook </param>
        /// <returns>
        /// Where Entry was routed to, if successful
        /// </returns>
        ///
        public IHttpActionResult Post(Entry newEntry)
        {
            if (!(isEntryValid(newEntry)))
            {
                return CreatedAtRoute("", new { id = Entry.INVALID_ID }, newEntry); ;
            }
            phoneBook.Add(newEntry);
            return CreatedAtRoute("DefaultApi", new { id = newEntry.Id }, newEntry);
        }

        /// <summary>
        /// Respond to DELETE request.
        /// Remove an existing entry requirement.
        /// </summary>
        /// <param name="id">The Entry id to delete</param>
        /// <returns>
        /// Delete request result
        /// </returns>
        public IHttpActionResult Delete(int id)
        {
            var itemToRemove = phoneBook.SingleOrDefault(r => r.Id == id);

            if (itemToRemove != null)
            {
                phoneBook.Remove(itemToRemove);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Respond to PUT request.
        /// Update an entry requirement.
        /// </summary>
        /// <param name="updateEntry">The entry to update in the phonebook </param>
        /// <param name="id">The Id of the entry to update </param>
        /// <returns>
        /// Result of PUT request
        /// </returns> 
        public IHttpActionResult Put(int id, Entry UpdateEntry)
        {
            Entry itemFound;

            if (!(isEntryValid(UpdateEntry)))
            {
                return Content(HttpStatusCode.NotAcceptable, UpdateEntry);
            }
            itemFound = phoneBook.SingleOrDefault(r => r.Id == id);

            if (itemFound != null)
            {
                phoneBook.Remove(itemFound);
                phoneBook.Add(UpdateEntry);
                return Content(HttpStatusCode.Accepted, UpdateEntry);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, UpdateEntry);
            }           
        }
    }
}
