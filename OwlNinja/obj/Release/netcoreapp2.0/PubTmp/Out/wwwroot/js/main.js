window.Router = Backbone.Router.extend({

    routes: {
        "": "home",
        "postsbytag/:tag": "postsbytag",
        "post/:url": "post",
        "aboutme": "aboutme",

        "admin": "admin",
        "settings": "settings",
        "createpost": "createpost",
        "editpost/:url": "editpost"
    },

    initialize: function () {
        this.headingView = new HeadingView();

        //Check for admin
        this.AdminToken = window.localStorage.getItem('admin-token');

        if (this.AdminToken != null)
            this.IsAdmin = true;
        else
            this.IsAdmin = false;
        var thisApp = this;
        //Get settings
        $.ajax({
            type: 'GET',
            url: '../api/admins/settings',
            dataType: 'json',
            async: false,
            success: function (data) {
                thisApp.Settings = data;
                $('#Loading').remove();
            }
        });

        
    },

    home: function () {

        $("#loading").removeClass('invisible');

        $('#PageHeaderContent').html(this.headingView.render(this.Settings.homeTitle, this.Settings.homeSubHeading, 'site-heading').el);
        this.headingView.setImage(this.Settings.homeBgUrl);
        var thisApp = this;
        $.ajax({
            type: 'GET',
            url: '../api/posts?skip=0',
            dataType: 'json',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (data) {
                var homeView = new HomeView();
                homeView.render(thisApp.IsAdmin, data);
                $("#Content").html(homeView.el);

                $("#loading").addClass('invisible');
            }
        });
    },

    postsbytag: function (tag) {
        $("#loading").removeClass('invisible');

        $('#PageHeaderContent').html(this.headingView.render(this.Settings.postsByTagsTitle, decodeURI(tag), 'site-heading').el);
        this.headingView.setImage(this.Settings.postsByTagsBgUrl);
        var thisApp = this;
        $.ajax({
            type: 'GET',
            url: '../api/posts?skip=0&tag=' + decodeURI(tag),
            dataType: 'json',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (data) {
                var homeView = new HomeView();
                homeView.setTag(decodeURI(tag));
                homeView.render(thisApp.IsAdmin, data);
                $("#Content").html(homeView.el);

                $("#loading").addClass('invisible');
            },
            error: function (jqXHR, exception) {
                $("#loading").addClass('invisible');
                app.navigate("", { trigger: true, replace: true });
            }
        });
    },

    post: function (url) {

        $("#loading").removeClass('invisible');

        var thisApp = this;
        $.ajax({
            type: 'GET',
            url: '../api/post?url=' + url,
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            dataType: 'json',
            success: function (data) {
                $('#PageHeaderContent').html(thisApp.headingView.render(data.postTitle, data.postSubHeading, 'post-heading').el);
                thisApp.headingView.setImage(data.headerPostImage);

                var postView = new PostView();
                postView.render(thisApp.IsAdmin, data, url);
                $("#Content").html(postView.el);

                $("#loading").addClass('invisible');
            },
            error: function (jqXHR, exception) {
                $("#loading").addClass('invisible');
                app.navigate("", { trigger: true, replace: true });
            }
        });
    },

    aboutme: function () {

        $("#loading").removeClass('invisible');

        $('#PageHeaderContent').html(this.headingView.render(this.Settings.aboutMeTitle, this.Settings.aboutMeSubHeading, 'page-heading').el);
        this.headingView.setImage(this.Settings.aboutMeBgUrl);

        this.aboutMeView = new AboutMeView();
        this.aboutMeView.render(this.Settings.aboutMeHtml);

        $('#Content').html(this.aboutMeView.el);

        $("#loading").addClass('invisible');
    },
    admin: function () {
        if (this.IsAdmin) {
            this.navigate("settings");
        }
        else
        {
            $("#loading").removeClass('invisible');

            $('#PageHeaderContent').html(this.headingView.render("Вход в админку","Требуется логин и пароль, которого, скорее всего у вас нет.", 'page-heading').el);
            this.headingView.setImage(this.Settings.adminImage);

            this.adminView = new AdminView();
            this.adminView.render();
            $("#Content").html(this.adminView.el);

            $("#loading").addClass('invisible');
        }
    },
    settings: function () {
        if (!this.IsAdmin) {
            this.navigate("admin");
        }
        else
        {
            $("#loading").removeClass('invisible');

            $('#PageHeaderContent').html(this.headingView.render("Настройки", "Чмоки всем.", 'page-heading').el);
            this.headingView.setImage(this.Settings.settingsImage);

            this.settingsView = new SettingsView();
            this.settingsView.render(this.Settings);
            $("#Content").html(this.settingsView.el);

            $("#loading").addClass('invisible');
        }
    },
    createpost: function () {
        if (!this.IsAdmin) {
            this.navigate("admin");
        }
        else {

            $("#loading").removeClass('invisible');

            $('#PageHeaderContent').html(this.headingView.render("Создание поста", "Чмоки всем.", 'page-heading').el);
            this.headingView.setImage(this.Settings.createPostImage);

            this.createPostView = new CreatePostView();
            this.createPostView.render();
            $("#Content").html(this.createPostView.el);

            $("#loading").addClass('invisible');
        }
    },
    editpost: function (url) {
        if (!this.IsAdmin) {
            this.navigate("admin");
        }
        else {

            $("#loading").removeClass('invisible');

            var thisApp = this;
            $.ajax({
                type: 'GET',
                url: '../api/post?url=' + url,
                dataType: 'json',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                success: function (data) {
                    $('#PageHeaderContent').html(thisApp.headingView.render("Редактирование поста", "Чмоки всем.", 'page-heading').el);
                    thisApp.headingView.setImage(thisApp.Settings.createPostImage);

                    this.editPostView = new EditPostView();
                    this.editPostView.render(data);
                    $("#Content").html(this.editPostView.el);

                    $("#loading").addClass('invisible');
                },
                error: function (jqXHR, exception) {
                    $("#loading").addClass('invisible');

                    window.localStorage.removeItem('admin-token');
                    app.IsAdmin = false;
                    app.navigate("", { trigger: true, replace: true });
                }
            });
        }
    }
});

templateLoader.load(["HomeView", "PostView", "AdminView", "AboutMeView", "HeadingView", "SettingsView", "CreatePostView","EditPostView"],
    function () {
        app = new Router();
        Backbone.history.start({ pushState: true });
    });