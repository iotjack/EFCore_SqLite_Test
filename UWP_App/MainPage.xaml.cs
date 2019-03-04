using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EFCore_SqLiteLib.Model;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace UWP_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //public ObservableCollection<Post> PostList = new ObservableCollection<Post>();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new BloggingContext())
            {
                Blogs.ItemsSource = db.Blogs.ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new BloggingContext())
            {
                var blog = new Blog { Url = NewBlogUrl.Text };
                blog.Posts = new List<Post>();
                blog.Posts.Add(new Post { Title = "post 0", Content = "content" });
                db.Blogs.Add(blog);
                db.SaveChanges();

                Blogs.ItemsSource = db.Blogs.ToList();
            }
        }

        private void AddPost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Blogs.SelectedItem != null)
                {
                    using (var db = new BloggingContext())
                    {
                        var selectedBlog = db.Blogs.Where(x => x.BlogId == ((Blog)Blogs.SelectedItem).BlogId).FirstOrDefault();
                           
                        if (selectedBlog.Posts == null)
                        {
                            selectedBlog.Posts = new List<Post>();
                        }
                        selectedBlog.Posts.Add(new Post { Title = NewPostTitle.Text, Content = "Hi Placeholder content" });
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void Blogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Blogs.SelectedItem != null)
                {
                    using (var db = new BloggingContext())
                    {
                        //var selectedBlog = db.Blogs.Include(x => x.Posts).Where(x => x.BlogId == ((Blog)Blogs.SelectedItem).BlogId).FirstOrDefault();

                        //var selectedBlog = (Blog)Blogs.SelectedItem;  //this is not tracked; no good;
                        var selectedBlog = db.Blogs.Single(b => b.BlogId == ((Blog)Blogs.SelectedItem).BlogId);

                        //explicit loading of navigation property
                        //db.Posts.Where(p => p.BlogId == selectedBlog.BlogId).Load();
                        db.Entry(selectedBlog).Collection(b => b.Posts).Load();
                        //var postCount = db.Entry(selectedBlog).Collection(b => b.Posts).Query().Count();
                        if (selectedBlog.Posts.Count>0)
                        {
                            Posts.ItemsSource = selectedBlog.Posts.ToList(); //

                            //to use this, setup ItemsSource in XAML
                            //PostList.Clear();
                            //foreach(var p in selectedBlog.Posts)
                            //{
                            //    PostList.Add(p);
                            //}
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
