(function () {
    "use strict"
    window.Interop = {
        getCookie: function (name) {
            let result = document.cookie.match("(^|[^;]+)\\s*" + name + "\\s*=\\s*([^;]+)");
            return result ? result.pop() : "";
        },
        getTokenDetails: function () {
            const tokenDetailsString = localStorage
                .getItem("auth-token-details");
            const tokenDetails = JSON.parse(tokenDetailsString ? tokenDetailsString : "{}");

            return tokenDetails;
        },
        setTokenDetails: function (tokenDetails) {
            localStorage.setItem(
                "auth-token-details",
                JSON.stringify(tokenDetails)
            );
        },
        getApplicationState: function () {
            const applicationStateString = localStorage
                .getItem("application-state");
            if (!applicationStateString) {
                return;
            }
            const applicationState = JSON.parse(applicationStateString);

            return applicationState;
        },
        setApplicationState: function (applicationState) {
            localStorage.setItem(
                "application-state",
                JSON.stringify(applicationState)
            );
        }
    };
})();