using System;

namespace eHesabim.Facebook.GraphApiEntities {
    /// <summary>
    /// Authorization options
    /// </summary>
    public class AutorizationOptions {
        /// <summary>
        /// Gets or sets a value indicating whether OfflineAccess.
        /// </summary>
        public bool OfflineAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether CreateEvent.
        /// </summary>
        public bool CreateEvent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RsvpEvent.
        /// </summary>
        public bool RsvpEvent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Sms.
        /// </summary>
        public bool Sms { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Email.
        /// </summary>
        public bool Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Birthday.
        /// </summary>
        public bool Birthday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ManagePages.
        /// </summary>
        public bool ManagePages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether XmppLogin.
        /// </summary>
        public bool XmppLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UserEvents.
        /// </summary>
        public bool UserEvents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ReadStream.
        /// </summary>
        public bool ReadStream { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ReadFriendlists.
        /// </summary>
        public bool ReadFriendlists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ReadRequests.
        /// </summary>
        public bool ReadRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ReadInsights.
        /// </summary>
        public bool ReadInsights { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PublishStream.
        /// </summary>
        public bool PublishStream { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UserNotes.
        /// </summary>
        public bool UserNotes { get; set; }

        /// <summary>
        /// Custom registration
        /// </summary>
        public void SetRegistration() {
            Email = true;
            Birthday = true;
        }

        /// <summary>
        /// Sets all options true
        /// </summary>
        public void SetAllOptionsTrue() {
            OfflineAccess = true;
            CreateEvent = true;
            RsvpEvent = true;
            Sms = true;
            Email = true;
            Birthday = true;
            ManagePages = true;
            XmppLogin = true;
            UserEvents = true;
            ReadFriendlists = true;
            ReadRequests = true;
            ReadInsights = true;
            PublishStream = true;
            UserNotes = true;
        }

        /// <summary>
        /// Returns option list
        /// </summary>
        /// <returns>Option List </returns>
        /// <exception cref="Exception"> not set </exception>
        public string GetListOfOptions() {
            var strOut = string.Empty;
            if (OfflineAccess) {
                strOut += "offline_access,";
            }

            if (CreateEvent) {
                strOut += "create_event,";
            }

            if (RsvpEvent) {
                strOut += "rsvp_event,";
            }

            if (Sms) {
                strOut += "sms,";
            }

            if (Email) {
                strOut += "email,";
            }

            if (Birthday) {
                strOut += "user_birthday,";
            }

            if (ManagePages) {
                strOut += "manage_pages,";
            }

            if (XmppLogin) {
                strOut += "xmpp_login,";
            }

            if (UserEvents) {
                strOut += "user_events,";
            }

            if (ReadStream) {
                strOut += "read_stream,";
            }

            if (ReadFriendlists) {
                strOut += "read_friendlists,";
            }

            if (ReadRequests) {
                strOut += "read_requests,";
            }

            if (ReadInsights) {
                strOut += "read_insights,";
            }

            if (PublishStream) {
                strOut += "publish_stream,";
            }

            if (UserNotes) {
                strOut += "user_notes,";
            }

            if (strOut == string.Empty) {
                throw new Exception("Must have at least one option when adding user to application");
            }

            strOut = strOut.Substring(0, strOut.Length - 1);
            return strOut;
        }
    }
}