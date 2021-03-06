# Team Project for the year 2019-20

## Team members

- Enrico Trombetta [2396702t](mailto:2396702t@student.gla.ac.uk)

- Liam Brodie [2198998b](mailto:2198998b@student.gla.ac.uk)

- Iacovos Kelepeniotis [2325969k](mailto:2325969k@student.gla.ac.uk)

- André Ferreira [2265787r](mailto:2265787r@student.gla.ac.uk)

- Erce Ozturk [2329415o](mailto:2329415o@student.gla.ac.uk)

# Setup

0. Install the Android SDK. You don't need android studio, but if you wish you can install it as well.

1. Download Unity Hub.

2. Create an account.

3. Install **Unity 2018.4.11f1.**

4. Clone the project and have it open by Unity Hub. You may get lots of import errors now, just ignore them at this stage.

5. Install ARCore SDK for Unity (https://developers.google.com/ar/develop/unity/quickstart-android) but do not open HelloAR.scene. Instead, open any of the scenes in `Asset/Scenes`.

6. Hopefully you should be able to launch a build now.

7. On your phone, install "Google Play Services for AR".

8. Install the generated apk at step 6 (either via adb or via issuing the 'build and run' command inside Unity).

# Project structure

The application is divided into a client, that is to say this Unity application, and a middleware.

The middleware is a Django application available [here](https://stgit.dcs.gla.ac.uk/tp3-2019-cs25/game-middleware),
and currently being hosted on [Heroku](http://glasgow-cs25-middleware.herokuapp.com/).
