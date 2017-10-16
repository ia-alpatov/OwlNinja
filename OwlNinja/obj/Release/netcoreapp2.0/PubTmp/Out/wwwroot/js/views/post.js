window.PostView = Backbone.View.extend({

    initialize: function () {
    },

    events: {
        "click .deleteButton": "deletePostClick",
        "click .editButton": "editPostClick",
        "click #sendCommentButton":"addComment"
    },

    render: function (IsAdmin,data,postUrl) {
        $(this.el).html(this.template({ IsAdmin: IsAdmin, post: data }));

        //load comments

        this.Url = postUrl;

        

        $.ajax({
            type: 'GET',
            url: '../api/comments/?url=' + postUrl,
            dataType: 'json',
            success: function (data) {

                var html = _.template('<%for(var comment in Comments){%><div class="row"><div class="col-lg-8 col-md-10 mx-auto"><p><%= Comments[comment].text %></p><p class="post-meta">Опубликовано в <%= Comments[comment].time %></p><%if(IsAdmin){%><p><button type="submit" style="margin:0 5px;" class="btn btn-warning float-left deleteCommentButton" data-id="<%= Comments[comment].id %>">Удалить комментарий</button></p><%}%></div></div><hr><%}%>', { IsAdmin: IsAdmin, Comments: data.comments });
                $('#Comments').append(html);

                $('.deleteCommentButton').each(function () {

                    var that = this;

                    $(this).click(function () {

                        if (confirm("Удалить комметарий?")) {
                            var form = new FormData();
                            form.append('id', $(that).attr("data-id"));

                            $.ajax({
                                type: 'DELETE',
                                url: '../api/admins/comment/',
                                processData: false,
                                contentType: false,
                                data: form,
                                headers: { "Authorization": "Bearer " + window.localStorage.getItem('admin-token') },
                                async: false,
                                success: function (data) {
                                    $(that).parent().parent().parent().remove();
                                },
                                error: function (jqXHR, exception) {
                                    window.localStorage.removeItem('admin-token');
                                    app.IsAdmin = false;
                                    app.navigate("", { trigger: true, replace: true });
                                }
                            });
                        }

                    });
                });
            }
        });

        return this;
    },
    addComment: function (ev) {


        var text = $('#commenttext').val();

           
            if (!text || text.length <= 1) {
                
                $('#CommentResult').html("Невозможно отправить пустой комментарий!");
                }
            else {
                var recaptcha = grecaptcha.getResponse(window.currentGrecaptchaId);
                    if (!recaptcha || recaptcha.length <= 3) {
                        $('#CommentResult').html("Решите reCAPTCHA!");
                    }
                    else {
                        $("#loading").removeClass('invisible');
                        $('#AuthResult').html("");

                        var form = new FormData();
                        form.append('url', this.Url);
                        form.append('text', text);
                        form.append('g-recaptcha-response', recaptcha);

                        var that = this;

                        $.ajax({
                            type: 'POST',
                            url: '../api/comments',
                            data: form,
                            processData: false,
                            contentType: false,
                            success: function (data) {
                                grecaptcha.reset(window.currentGrecaptchaId);

                                $("#loading").addClass('invisible');

                                location.reload();
                            },
                            error: function (jqXHR, exception) {
                                grecaptcha.reset(window.currentGrecaptchaId);

                                $('#AuthResult').html("Невозможно отправить комментарий.");

                                $("#loading").addClass('invisible');
                            },
                        });
                    }
                }
            

    },

    deletePostClick: function (ev) {
        if (confirm("Удалить пост: " + $(ev.target).attr("data-id") + "?")) {
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
                success: function (data) {
                    app.navigate("", { trigger: true, replace: true });
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
