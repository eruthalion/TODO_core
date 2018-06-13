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


        public bool SaveTask(Models.Model.Task newTask, string ownerEmail, List<string> users)
        {
            bool retval = true;
            Model.User actuser = _model.Users.Where(z => z.Email == ownerEmail).FirstOrDefault();
            if (actuser == null)
            {
                actuser=SaveUser(ownerEmail);
              
            }

            List<Model.User> userList = _model.Users.Where(z => users.Contains(z.Email)).ToList();

            foreach(string x in users)
            {
                if (_model.Users.Where(z => z.Email== x).FirstOrDefault()==null)
                {
                    userList.Add(SaveUser(x));
                }
            }

            Model.TaskUser task2user = new Model.TaskUser();
            var task = newTask;
            var user = actuser;
            List<Model.TaskUser> taskUserList = new List<Model.TaskUser>();
            foreach(Model.User y in userList)
            {
                taskUserList.Add(new Model.TaskUser { Task = task, User = y });
            }
            task.Users = taskUserList;
            task.Owner = actuser;
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
                _model.Users.Add(retval);
                _model.SaveChanges();
            }
            catch
            {

            }

            return retval;
        }

        internal bool UpdateTask(int id, int actuserId, Model.Task modTask, string taskOwnerEmail, List<string> users)
        {
            bool retval = true;
            Model.Task actTask = _model.Tasks.Where(t => t.Id == id).Include(u => u.Users).ThenInclude(k => k.User).FirstOrDefault();
            if (actuserId == actTask.Owner.Id)
            {

                List<Model.TaskUser> actTaskUserToRemove = actTask.Users.Where(z => !users.Contains(z.User.Email)).ToList();
                foreach (Model.TaskUser u in actTaskUserToRemove)
                {
                    actTask.Users.Remove(u);
                }
                actTask.Name = modTask.Name;
                actTask.CreateDate = modTask.CreateDate;
                actTask.NotifyTime = modTask.NotifyTime;
            }
            else
            {
                retval = false;
            }


            return retval;
        }
    }
}
