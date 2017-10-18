using System.Collections.Generic;

namespace eHesabim.Facebook.GraphApiEntities {
    /// <summary>
    /// Wall post
    /// </summary>
    public class WallPost {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets From.
        /// </summary>
        public Person From { get; set; }

        /// <summary>
        /// Gets or sets To.
        /// </summary>
        public Person To { get; set; }

        /// <summary>
        /// Gets or sets Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets Actions.
        /// </summary>
        public List<Action> Actions { get; set; }

        /// <summary>
        /// Gets or sets Privacy.
        /// </summary>
        public Privacy Privacy { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets CreatedTime.
        /// </summary>
        public string CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets UpdatedTime.
        /// </summary>
        public string UpdatedTime { get; set; }
    }
}