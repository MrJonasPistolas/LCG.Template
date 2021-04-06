var getBodyElement = function () {
    return document.getElementsByTagName('body')[0];
}

var addCssClassToBody = function (cssClass) {
    var body = getBodyElement();

    if (!body.classList.contains(cssClass)) {
        body.classList.add(cssClass);
    }
}