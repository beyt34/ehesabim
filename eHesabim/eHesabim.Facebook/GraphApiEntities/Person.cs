using System;

namespace eHesabim.Facebook.GraphApiEntities {
    /// <summary>
    /// Facebook person class
    /// </summary>
    public class Person {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets firstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets birthday.
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Gets or sets website.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets timezone.
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Gets or sets locale.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets verified.
        /// </summary>
        public string Verified { get; set; }

        /// <summary>
        /// Gets or sets updatedTime.
        /// </summary>
        public string UpdatedTime { get; set; }

        /// <summary>
        /// Gets or sets gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string Email { get; set; }
    }

    ////   "id": "533815668",
    ////   "name": "Alex VanLaningham",
    ////   "first_name": "Alex",
    ////   "last_name": "VanLaningham",
    ////   "link": "http://www.facebook.com/avanlaningham",
    ////   "birthday": "09/05/1966",
    ////   "website": "http://www.crewtrumpet.com",
    ////   "timezone": 2,
    ////   "locale": "en_US",
    ////   "verified": true,
    ////   "updated_time": "2010-04-08T06:51:17+0000"
}
