window.HomeView = Backbone.View.extend({

    initialize:function () {
    },

    events:{
        "click .deleteButton": "deletePostClick",
        "click .editButton": "editPostClick"
    },

    render: function (IsAdmin, data) {
        $(this.el).html(this.template({ IsAdmin: IsAdmin, Posts: data.posts }));

        this.IsAdmin = IsAdmin;
        this.CountLeft = data.CountLeft;
        this.Skip = 10;

        var view = this;

        $(window).scroll(function () {
            $('#LoadingText').each(function () {
                if (view.isScrolledIntoView($(this))) {

                    if (view.CountLeft > 0)
                    {
                        $(this).text('Загрузка...');

                        $.ajax({
                            type: 'GET',
                            url: '../api/posts?skip=' + view.Skip + (view.Tag != null) ? '&tag=' + view.Tag : '',
                            dataType: 'json',
                            success: function (data) {
                                view.Skip = view.Skip + 10;
                                view.CountLeft = data.CountLeft;

                                var html = _.template(' <% for(var post in Posts){ %> <div class="post-preview"> <a href="/post/<%= Posts[post].enTitle %>"> <h2 class="post-title"> <%= Posts[post].postTitle %> </h2> <h3 class="post-subtitle"> <%= Posts[post].postSubHeading %> </h3> </a> <p class="post-meta"> Опубликовано в <%= Posts[post].postDate %> </p> <p class="post-meta"> <% for(var tag in Posts[post].tags){ %> <a style="margin:0 5px;" href="/postsbytag/<%= tag %>"><%= Posts[post].tags[tag] %></a> <% } %> </p> <% if(IsAdmin) { %> <p class="post-meta"> <button type="submit" class="btn btn-secondary float-left deleteButton" data-id="<%= Posts[post].enTitle %>">Удалить пост</button> <button type="submit" class="btn btn-secondary float-right editButton" data-id="<%= Posts[post].enTitle %>">Редактировать пост</button> </p> <% } %> </div> <hr> <% } %>', { IsAdmin: view.IsAdmin, Posts: data.posts });
                                $('#AfterPostContainer').before(html);

                                $(this).text('');
                            }
                        });

                    } 
                    else
                        $(this).text('Всё загружено!');
                }
                else
                {
                    $(this).text('');
                }
            });
        });

        return this;
    },

    isScrolledIntoView: function (elem) {
        var $elem = $(elem);
        var $window = $(window);

        var docViewTop = $window.scrollTop();
        var docViewBottom = docViewTop + $window.height();

        var elemTop = $elem.offset().top;
        var elemBottom = elemTop + $elem.height();

        return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
    },

    setTag: function (tag) {
        this.Tag = tag;
    },

    deletePostClick: function (ev) {
        if (confirm("Удалить пост: " + $(ev.target).attr("data-id")+"?"))
        {
            var form = new FormData();
            form.append('url', $(ev.target).attr("data-id"));

            $.ajax({
                type: 'DELETE',
                url: '../api/admins/post/',
                processData: false,
                contentType: false,
                data: form,
                headers: { "Authorization": "Bearer " + window.localStorage.getItem('admin-token') },
                async: false,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                success: function (data) {
                    if (this.Tag != null) {
                       app.navigate("postsbytag/" + this.Tag, { trigger: true, replace: true }); 
                    }
                    else
                    {
                       app.navigate("", { trigger: true, replace: true }); 
                    }
                },
                error: function (jqXHR, exception) {
                    window.localStorage.removeItem('admin-token');
                    app.IsAdmin = false;
                    app.navigate("", { trigger: true, replace: true });
                }
            });
        }
    },

    editPostClick: function (ev) {
       app.navigate("editpost/" + $(ev.target).attr("data-id"), { trigger: true }); 
    }

});