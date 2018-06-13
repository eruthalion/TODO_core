using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace todo_gabor.Models
{
    public class Management
    {
        private readonly Models.Model.ModelContainer _model;

        public Management(Model.ModelContainer model)
        {
            _model = model;
        }

        public List<Model.Task> GetTasks(int id=0, bool forUser=false)
        {
            List<Model.Task> tasks = new List<Model.Task>();
            if (id == 0)
            {
                tasks.AddRange(_model.Tasks.Include(t => t.Users));
            }
            else
            {
                if (forUser)
                {                   



                    tasks.AddRange(_model.Tasks.Where(z => z.Users.Select(t => t.UserId).Contains(id)).Include(u => u.Users).ThenInclude(k=>k.User).ToList());
                }
                else
                {
                    tasks.Add(_model.Tasks.Include(t => t.Users).FirstOrDefault(z => z.Id == id));
                }
            }

            return tasks;
        }


        public bool SaveTask(Models.Model.Task newTask, string ownerEmail)
        {
            bool retval = true;
            Models.Model.User actuser = _model.Users.Where(z => z.Email == ownerEmail).FirstOrDefault();
            if (actuser == null)
            {
                actuser=SaveUser(ownerEmail);
                _model.Users.Add(actuser);
            }

            Model.TaskUser task2user = new Model.TaskUser();
            //ver 1
            //task2user.User = actuser;
            //if (newTask.Users == null)
            //{
            //    newTask.Users = new List<Model.TaskUser>();
            //}
            //if (actuser.Tasks == null)
            //{
            //    actuser.Tasks = new List<Model.TaskUser>();
            //}

            //newTask.Users.Add(task2user);

            //actuser.Tasks.Add(task2user);
            //newTask.Users.Add(task2user);
            //_model.Tasks.Add(newTask);

            //ver 2

            //var task = newTask;
            //var user = actuser;
            //var taskuser = new Model.TaskUser {Task= task, User= user };

            //_model.Tasks.Add(newTask);
            //_model.SaveChanges();
            //_model.TaskUser.Add(taskuser);

            //ver 3

            var task = newTask;
            var user = actuser;
            task.Users = new List<Model.TaskUser>
            {
                new Model.TaskUser
                {
                    Task=task,
                    User=user
                }
            };
            _model.Tasks.Add(task);
            
            _model.SaveChanges();



            return retval;
        }

        public Model.User SaveUser(string email, string password="asd123ASD")
        {
            Model.User retval = new Model.User();
            try
            {
              
                retval.Email = email;
                retval.Password = password;
                retval.isOwner = true;
                _model.Users.Add(retval);
                _model.SaveChanges();
            }
            catch
            {

            }

            return retval;
        }

    }
}
