﻿using ExamProjectOne.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamProjectOne.Models
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public string Skill { get; set; } = string.Empty;
        [Required]
        public string ShiftTime { get; set; } = string.Empty;
        [Required]
        public string WorkDay { get; set; } = string.Empty;
        [Required]
        public List<Schedule> Schedules { get; set; } = [];
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
