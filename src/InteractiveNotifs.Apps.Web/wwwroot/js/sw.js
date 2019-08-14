/*
*
*  Push Notifications codelab
*  Copyright 2015 Google Inc. All rights reserved.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*      https://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License
*
*/

/* eslint-env browser, serviceworker, es6 */

'use strict';

self.addEventListener('push', function (event) {
    console.log('[Service Worker] Push Received.');
    console.log(`[Service Worker] Push had this data: "${event.data.text()}"`);

    var webNotif = JSON.parse(event.data.text());


    const title = webNotif.Title;
    const options = {
    };

    if (webNotif.Body) {
        options.body = webNotif.Body;
    }

    if (webNotif.Image) {
        options.image = webNotif.Image;
    }

    if (webNotif.Actions && webNotif.Actions.length > 0) {
        options.actions = [];
        webNotif.Actions.forEach(action => {
            options.actions.push({
                title: action.Title,
                action: action.Action
            });
        });
    }

    event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener('notificationclick', function (event) {
    console.log('[Service Worker] Notification click Received.');

    event.notification.close();

    if (event.action) {
        try {
            var cmd = JSON.parse(event.action);
            if (cmd.Type === 'Command.OpenUrl') {
                event.waitUntil(
                    clients.openWindow(cmd.Url)
                );
                return;
            } else if (cmd.Type === 'Command.Http') {
                var method = 'GET';
                if (cmd.Method) {
                    method = cmd.Method;
                }

                event.waitUntil(
                    fetch(cmd.Url, { method: method })
                        .then(response => {
                            return response.text();
                        }).then(txt => {
                            return self.registration.showNotification('HTTP response', { body: txt });
                        })
                );
                return;
            }
        }
        catch (ex) { console.log('[Service Worker] Failed handling action ' + event.action); }
    }


    event.waitUntil(
        clients.openWindow('https://developers.google.com/web/')
    );

});