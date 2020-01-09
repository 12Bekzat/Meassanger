using System;

namespace Messanger
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; }
    }
}