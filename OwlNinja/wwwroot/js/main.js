window.Router = Backbone.Router.extend({

    routes: {
        "": "home",
        "postsbytag/:tag": "postsbytag",
        "post/:url": "post",
        "aboutme": "aboutme",

        "admin": "admin",
        "settings": "settings",
        "createpost": "createpost"
    },

    initialize: function () {
        this.headingView = new HeadingView();

        //Check for admin
        this.AdminToken = window.localStorage.getItem('admin-token');

        if (this.AdminToken != null)
            this.IsAdmin = true;
        else
            this.IsAdmin = false;

        //Get settings
        $.ajax({
            type: 'GET',
            url: '../api/admin/settings',
            dataType: 'json',
            async: false,
            success: function (data) {
                window.Router.Settings = data;
                $('#Loading').remove();
            }
        });

        
    },

    home: function () {

        $('#PageHeaderContent').html(this.headingView.render(this.Settings.HomeTitle, this.Settings.HomeSubHeading, 'site-heading').el);
        this.headerView.setImage(this.Settings.HomeBgUrl);

        $.ajax({
            type: 'GET',
            url: '../api/posts?skip=0',
            dataType: 'json',
            success: function (data) {
                var homeView = new HomeView();
                homeView.render(window.Router.IsAdmin, data);
                $("#Content").html(homeView.el);
            }
        });
    },

    postsbytag: function (tag) {
        $('#PageHeaderContent').html(this.headingView.render(this.Settings.PostsByTagsTitle, tag, 'site-heading').el);
        this.headerView.setImage(this.Settings.PostsByTagsBgUrl);

        $.ajax({
            type: 'GET',
            url: '../api/tags?skip=0&tag='+tag,
            dataType: 'json',
            success: function (data) {
                var homeView = new HomeView();
                homeView.setTag(tag);
                homeView.render(window.Router.IsAdmin, data);
                $("#Content").html(homeView.el);
            }
        });
    },

    post: function (url) {
        $.ajax({
            type: 'GET',
            url: '../api/posts?url=' + url,
            dataType: 'json',
            success: function (data) {
                $('#PageHeaderContent').html(window.Router.headingView.render(data.PostTitle, data.PostSubHeading, 'post-heading').el);
                this.headerView.setImage(data.HeaderPostImage);

                var postView = new PostView();
                postView.render(window.Router.IsAdmin, data);
                $("#Content").html(this.postView.el);
            }
        });
    },

    aboutme: function () {
        $('#PageHeaderContent').html(this.headingView.render(this.Settings.AboutMeTitle, this.Settings.AboutMeSubHeading, 'page-heading').el);
        this.headerView.setImage(this.Settings.AboutMeBgUrl);

        if (!this.aboutMeView) {
            this.aboutMeView = new AboutMeView();
            this.aboutMeView.render(this.Settings);
        }

        $('#Content').html(this.aboutMeView.el);
    },
    admin: function () {
        if (this.IsAdmin) {
            this.navigate("settings");
        }
        else
        {
            $('#PageHeaderContent').html(this.headingView.render("Вход в админку","Требуется пароль и логин, которого, скорее всего у вас нет.", 'page-heading').el);
            this.headerView.setImage(this.Settings.AdminImage);

            this.adminView = new AdminView();
            this.adminView.render();
            $("#Content").html(this.postView.el);
        }
    },
    settings: function () {
        if (!this.IsAdmin) {
            this.navigate("");
        }
        else
        {
            $('#PageHeaderContent').html(this.headingView.render("Настройки", "Чмоки всем.", 'page-heading').el);
            this.headerView.setImage(this.Settings.SettingsImage);

            this.settingsView = new SettingsView();
            this.settingsView.render(this.Settings);
            $("#Content").html(this.postView.el);
        }
    },
    createpost: function () {
        if (!this.IsAdmin) {
            this.navigate("");
        }
        else {
            $('#PageHeaderContent').html(this.headingView.render("Создание поста", "Чмоки всем.", 'page-heading').el);
            this.headerView.setImage(this.Settings.CreatePostImage);

            this.createPostView = new CreatePostView();
            this.createPostView.render();
            $("#Content").html(this.postView.el);
        }
    }
});

templateLoader.load(["HomeView", "PostView", "CommentsView", "AdminView","AboutMeView", "HeadingView","SettingsView","CreatePostView"],
    function () {
        app = new Router();
        Backbone.history.start();
    });