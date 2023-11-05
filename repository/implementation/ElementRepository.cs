using Patterson.exception;
using Patterson.model;
using Patterson.persistence;
using Patterson.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Patterson.repository.implementation
{
    public class ElementRepository : IElementRepository
    {
        private readonly PattersonDBContext context;

        public ElementRepository(PattersonDBContext context)
        {
            this.context = context;

        }

        public string[] GetAllElementNames()
        {
            string[] elementNames = context.Elements.Select(e => e.Name).ToArray();
            return elementNames;
        }

        public Element FindElementByName(string name)
        {
            Element element = context.Elements.FirstOrDefault(e => e.Name == name);
            return element;
        }
    }
}
