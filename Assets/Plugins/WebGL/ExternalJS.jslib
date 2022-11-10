mergeInto(LibraryManager.library, {
  // override default callback
  emscripten_set_wheel_callback_on_thread: function (
    target,
    userData,
    useCapture,
    callbackfunc,
    targetThread
  ) {
    target = findEventTarget(target);
 
    // the fix
    if (!target) {
      return -4;
    }
 
    if (typeof target.onwheel !== 'undefined') {
      registerWheelEventCallback(
        target,
        userData,
        useCapture,
        callbackfunc,
        9,
        'wheel',
        targetThread
      );
      return 0;
    } else {
      return -1;
    }
  },
  HelloString: function (str) {
    ReactUnityWebGL.HelloString(UTF8ToString(str));
  },
  UnityReady: function () {
	ReactUnityWebGL.UnityReady();
  },
});