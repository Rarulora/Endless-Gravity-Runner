mergeInto(LibraryManager.library, {
  GR_SyncFS_In: function () { 
    FS.syncfs(true, function (err) {
      if (err) console.error('GR_SyncFS_In error:', err);
    });
  },
  GR_SyncFS_Out: function () {
    FS.syncfs(false, function (err) {
      if (err) console.error('GR_SyncFS_Out error:', err);
    });
  }
});
