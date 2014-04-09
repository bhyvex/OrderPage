$.postJSON = function(url, data, callback) {
    $.post(url, data, callback, "json");
}; 

function TrimSpace(passedString) {
    if (typeof (passedString) != 'undefined' && passedString != '') {
        while (passedString[0] == ' ') {
            passedString = passedString.toString().substring(1);
        }
        while (passedString[passedString.length - 1] == ' ') {
            passedString = passedString.toString().substring(0, passedString.length - 1);
        }
        return passedString;
    } else {
        return '';
    }
}
