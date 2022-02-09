# [1.2.0](https://github.com/f1yingbanana/sfizz-unity/compare/1.1.0...1.2.0) (2022-02-09)


### Bug Fixes

* add assertions so Sfizz logs an error instead of crashing Unity ([fde8076](https://github.com/f1yingbanana/sfizz-unity/commit/fde8076f1917d4a29754edf0ab90f97266c3f54e))
* SfizzPlayer samples generated per frame no longer exceeds current buffer size ([baeed55](https://github.com/f1yingbanana/sfizz-unity/commit/baeed55326fb5e75f901fffd7365b0b84c3e1189))


### Features

* add a MIDI file renderer that is capable of converting MIDI files to  AudioClips ([f871204](https://github.com/f1yingbanana/sfizz-unity/commit/f871204d2327fa33bd402f9d3350be00ffc8639e))
* add a MIDI player sample that showcases SfizzMidiRenderer ([e3e9795](https://github.com/f1yingbanana/sfizz-unity/commit/e3e9795af3ac57ea4b3e229645ff783f66f9a273))

# [1.1.0](https://github.com/f1yingbanana/sfizz-unity/compare/1.0.0...1.1.0) (2022-02-04)


### Bug Fixes

* add correction for negative latency in AudioSource playback ([7065406](https://github.com/f1yingbanana/sfizz-unity/commit/7065406148bcf3b8c438042cb5cd133ec74779d2))
* replace string path for sfz files with file object path in piano sample ([950566b](https://github.com/f1yingbanana/sfizz-unity/commit/950566bbbcc0f70375e5e4d266ac06c489131812))


### Features

* add iOS support ([f579862](https://github.com/f1yingbanana/sfizz-unity/commit/f579862d7141af632e8b9a3ba8492f835710afc2))

# 1.0.0 (2022-02-04)


### Features

* add piano sample ([1062b80](https://github.com/f1yingbanana/sfizz-unity/commit/1062b806ec39f8446d475cf1d400e8be2e60132a))
* create wrapper for sfizz.h ([d2a99b8](https://github.com/f1yingbanana/sfizz-unity/commit/d2a99b8a29609bc3145797e82d8567a336a6af1b))
* implement a streaming audio player ([c4c5b5d](https://github.com/f1yingbanana/sfizz-unity/commit/c4c5b5d93d761fc319f98a48d4cca41dc7be53e0))
