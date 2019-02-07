using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ProxySubExamples
    {
        public class TemplateMethod
        {
            public class FormResult
            {
                public bool IsValid { get; set; }
                public bool IsComplete { get; set; }
            }

            public abstract class Form
            {
                public virtual PartialSubExamples.TemplateMethod.FormResult Submit()
                {
                    var result = Validate();
                    if (result.IsValid)
                    {
                        Save();
                        result.IsComplete = true;
                    }
                    return result;
                }

                public abstract PartialSubExamples.TemplateMethod.FormResult Validate();
                public abstract void Save();
            }

            [Test]
            public void ValidFormSubmission()
            {
                var form = Substitute.ForPartsOf<PartialSubExamples.TemplateMethod.Form>();
                form.Validate().Returns(new PartialSubExamples.TemplateMethod.FormResult() { IsValid = true });

                var result = form.Submit();

                Assert.That(result.IsComplete);
                form.Received().Save();
            }

            [Test]
            public void InvalidFormSubmission()
            {
                var form = Substitute.ForPartsOf<PartialSubExamples.TemplateMethod.Form>();
                form.Validate().Returns(new PartialSubExamples.TemplateMethod.FormResult() { IsValid = false });

                var result = form.Submit();

                Assert.That(result.IsComplete, Is.False);
                form.DidNotReceive().Save();
            }
        }

        public class TemplateMethod2Example
        {
            public class SummingReader
            {
                public virtual int Read()
                {
                    var s = ReadFile();
                    return s.Split(',').Select(int.Parse).Sum();
                }
                public virtual string ReadFile() { return "the result of reading the file here"; }
            }

            [Test]
            public void ShouldSumAllNumbersInFile()
            {
                var reader = Substitute.ForPartsOf<PartialSubExamples.TemplateMethod2Example.SummingReader>();
                reader.ReadFile().Returns("1,2,3,4,5");

                var result = reader.Read();

                Assert.That(result, Is.EqualTo(15));
            }
        }

        public class UnderlyingListExample
        {
            public class TaskList
            {
                readonly List<string> list = new List<string>();
                public virtual void Add(string s) { list.Add(s); }
                public virtual string[] ToArray() { return list.ToArray(); }
            }

            public class TaskView
            {
                private readonly PartialSubExamples.UnderlyingListExample.TaskList _tasks;
                public string TaskEntryField { get; set; }
                public string[] DisplayedTasks { get; set; }

                public TaskView(PartialSubExamples.UnderlyingListExample.TaskList tasks) { _tasks = tasks; }

                public void ClickButton()
                {
                    _tasks.Add(TaskEntryField);
                    DisplayedTasks = _tasks.ToArray();
                }
            }

            [Test]
            public void AddTask()
            {
                var list = Substitute.ForPartsOf<PartialSubExamples.UnderlyingListExample.TaskList>();
                var view = new PartialSubExamples.UnderlyingListExample.TaskView(list);
                view.TaskEntryField = "write example";

                view.ClickButton();

                // list substitute functions as test spy
                list.Received().Add("write example");
                Assert.That(view.DisplayedTasks, Is.EqualTo(new[] { "write example" }));
            }
        }
    }
}