window.HeadingView = Backbone.View.extend({

    initialize: function () {
        
    },
    render: function (title, subheading, type) {
        $('#PageHeaderContent').html(this.template({ Title: title, SubHeading: subheading, PageClass:type}));
        return this;
    },

    setImage: function (image) {
        $('#PageHeader').attr('style', "background-image: url('" + image + "')");
    }
});