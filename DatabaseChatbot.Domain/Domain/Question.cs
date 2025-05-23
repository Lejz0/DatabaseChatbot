﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain
{
    public class Question : BaseEntity
    {
        public Guid? DatabaseId { get; set; }

        public Database? Database { get; set; }
        public string QuestionText { get; set; }
        public string? QuestionAnswer { get; set; }
    }
}
