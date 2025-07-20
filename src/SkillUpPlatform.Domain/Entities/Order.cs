using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillUpPlatform.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // لو عايزة تربطي الأوردر بالمستخدم
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
    }
}
