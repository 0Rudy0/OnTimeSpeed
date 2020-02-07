using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Models
{
    public enum WorkItemType
    {
        Feature,
        Task,
        Incident,
        Defect,
        Item
    }

    public class WorkItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WorkItemType Type { get; set; }
    }


    public class WorkItemRaw
    {
        public Status status { get; set; }
        public int id { get; set; }
        public string item_type { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public bool has_attachments { get; set; }
        public Subitems subitems { get; set; }
        public Project project { get; set; }
        public Parent parent { get; set; }
        public Workflow_Step workflow_step { get; set; }
        public bool has_related_items { get; set; }
    }

    public class Status
    {
        public string name { get; set; }
        public int order { get; set; }
        public int id { get; set; }
    }

    public class Subitems
    {
        public int count { get; set; }
    }

    public class Parent
    {
        public int id { get; set; }
    }

    public class Workflow_Step
    {
        public string name { get; set; }
        public int order { get; set; }
        public int id { get; set; }
    }


}