﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class ChevronsProcessor_Testing
    {
        [Test]
        public void DoubleQuotation()
        {
            PostPartBase[] parts =
            {
                new RawTextPostPart(">A"),  LineBreakPart.Instance,
                new RawTextPostPart(">B"),  LineBreakPart.Instance,
                new RawTextPostPart(">>C"), LineBreakPart.Instance,
                new RawTextPostPart(">D"),
            };

            PostPartBase[] expected =
            {
                new ParagraphStartPart(1),
                new RawTextPostPart("A"),   
                new RawTextPostPart("B"),  
                new ParagraphStartPart(2),
                new RawTextPostPart("C"),
                new ParagraphStartPart(1),
                new RawTextPostPart("D"),
            };

            Check(parts, expected);
        }

        [Test]
        public void SimplestCase()
        {
            PostPartBase[] parts =
            {
                new RawTextPostPart(">A")
            };

            PostPartBase[] expected =
            {
                new ParagraphStartPart(1),
                new RawTextPostPart("A")
            };

            Check(parts, expected);
        }

        private void Check(PostPartBase[] parts, PostPartBase[] expected)
        {
            IProcessor cp = new ChevronsProcessor2();
            List<PostPartBase> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }
    }
}
