﻿using API.DTOs;

namespace API.Helpers
{
    public class MessageParams : PaginationParam
    {
        public string? Username { get; set; }
        public string Container { get; set; } = "Unread";


    }
}
