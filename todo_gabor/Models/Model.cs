using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todo_gabor.Models
{
    public class Model
    {
       public class User
        {
            [Key]
            public int Id { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }         
            public virtual ICollection<Task> OwnTasks { get; set; }
            public virtual ICollection<TaskUser> Tasks { get; set; }

        }

       public class Task
        {
            [Key]
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreateDate { get; set; }
            public DateTime NotifyTime { get; set; }
            public virtual User Owner { get; set; }
            public virtual ICollection<TaskUser> Users { get; set; }

        }

        public class TaskUser
        {
            public int UserId { get; set; }
            public virtual User User { get; set; }

            public int TaskId { get; set; }
            public virtual Task Task { get; set; }
        }

        public class ModelContainer : DbContext
        {
            public ModelContainer(DbContextOptions<ModelContainer> options)
            : base(options)
            {

            }



            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .HasKey(x => x.Id);

                modelBuilder.Entity<Task>()
                    .HasKey(x => x.Id);

                modelBuilder.Entity<TaskUser>()
                .HasKey(x => new { x.UserId, x.TaskId });
                modelBuilder.Entity<TaskUser>()
                    .HasOne(x => x.User)
                    .WithMany(m => m.Tasks)
                    .HasForeignKey(x => x.UserId);
                modelBuilder.Entity<TaskUser>()
                    .HasOne(x => x.Task)
                    .WithMany(e => e.Users)
                    .HasForeignKey(x => x.TaskId);

            }

            public DbSet<User> Users { get; set; }
            public DbSet<Task> Tasks { get; set; }
            public DbSet<TaskUser> TaskUser { get; set; }
        }
    }
}
