﻿/* Copyright 2010-2012 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MongoDB.Bson.Serialization.Conventions;

namespace MongoDB.BsonUnitTests.Serialization.Conventions
{
    [TestFixture]
    public class ElementNameConventionsTests
    {
        private class TestClass
        {
            public string FirstName { get; set; }
            public int Age { get; set; }
            public string _DumbName { get; set; }
            public string lowerCase { get; set; }
        }

        [Test]
        public void TestMemberNameElementNameConvention()
        {
#pragma warning disable 0618
            var convention = new MemberNameElementNameConvention();
#pragma warning restore 0618
            Assert.AreEqual("FirstName", convention.GetElementName(typeof(TestClass).GetProperty("FirstName")));
            Assert.AreEqual("Age", convention.GetElementName(typeof(TestClass).GetProperty("Age")));
            Assert.AreEqual("_DumbName", convention.GetElementName(typeof(TestClass).GetProperty("_DumbName")));
            Assert.AreEqual("lowerCase", convention.GetElementName(typeof(TestClass).GetProperty("lowerCase")));
        }

        [Test]
        public void TestCamelCaseElementNameConvention()
        {
#pragma warning disable 0618
            var convention = new CamelCaseElementNameConvention();
#pragma warning restore 0618
            Assert.AreEqual("firstName", convention.GetElementName(typeof(TestClass).GetProperty("FirstName")));
            Assert.AreEqual("age", convention.GetElementName(typeof(TestClass).GetProperty("Age")));
            Assert.AreEqual("_DumbName", convention.GetElementName(typeof(TestClass).GetProperty("_DumbName")));
            Assert.AreEqual("lowerCase", convention.GetElementName(typeof(TestClass).GetProperty("lowerCase")));
        }
    }
}
