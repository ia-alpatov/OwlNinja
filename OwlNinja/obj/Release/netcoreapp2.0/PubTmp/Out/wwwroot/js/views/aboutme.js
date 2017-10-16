window.AboutMeView = Backbone.View.extend({

    initialize: function () {
    },

    render: function (aboutMeHtml) {
        $(this.el).html(this.template({ AboutHtml: aboutMeHtml}));
        return this;
    }

});