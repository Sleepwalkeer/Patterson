namespace Patterson.Migrations
{
    using Patterson.model;
    using Patterson.utils;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Patterson.persistence.PattersonDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Patterson.persistence.PattersonDBContext context)
        {
            if (!context.Elements.Any())
            {
                List<Element> elements = PropertyReader.getAllElements();
                context.Elements.AddRange(elements);
                context.SaveChanges();
            }
        }
    }
}
