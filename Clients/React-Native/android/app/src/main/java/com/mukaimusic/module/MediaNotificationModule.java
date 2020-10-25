package com.mukaimusic.module;


import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.Icon;
import android.media.session.MediaSession;
import android.os.AsyncTask;
import android.os.Build;
import android.support.v4.media.session.MediaSessionCompat;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.mukaimusic.R;

import org.jetbrains.annotations.NotNull;

import java.io.BufferedInputStream;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;


public class MediaNotificationModule extends ReactContextBaseJavaModule {
    public static final String ACTION_NEXT = "com.mukaimusic.module.ACTION_NEXT";
    public static final String ACTION_PRE = "pre";
    public static final String ACTION_PAUSE = "pause";
    public static final String ACTION_PLAY = "play";
    private static final String CHANNEL_ID = "com.mukaimusic.CHANNEL";
    private static final int NOTIFICATION_ID = 0;
    private static ReactApplicationContext reactContext;
    private final PendingIntent preIntent;
    private final PendingIntent nextIntent;
    private final PendingIntent pauseIntent;
    private final PendingIntent playIntent;

    public MediaNotificationModule(ReactApplicationContext reactContext) {
        super(reactContext);
        MediaNotificationModule.reactContext = reactContext;

        ControlReceiver receiver = new ControlReceiver();
        IntentFilter filter = new IntentFilter();
        filter.addAction(ACTION_NEXT);
        filter.addAction(ACTION_PRE);
        filter.addAction(ACTION_PAUSE);
        filter.addAction(ACTION_PLAY);
        reactContext.registerReceiver(receiver, filter);

        Intent _nextIntent = new Intent(ACTION_NEXT);
        nextIntent = PendingIntent.getBroadcast(reactContext, 0, _nextIntent, PendingIntent.FLAG_UPDATE_CURRENT);

        Intent _preIntent = new Intent(ACTION_PRE);
        preIntent = PendingIntent.getBroadcast(reactContext, 0, _preIntent, PendingIntent.FLAG_UPDATE_CURRENT);

        Intent _pauseIntent = new Intent(ACTION_PAUSE);
        pauseIntent = PendingIntent.getBroadcast(reactContext, 0, _pauseIntent, PendingIntent.FLAG_UPDATE_CURRENT);

        Intent _playIntent = new Intent(ACTION_PLAY);
        playIntent = PendingIntent.getBroadcast(reactContext, 0, _playIntent, PendingIntent.FLAG_UPDATE_CURRENT);


        createNotificationChannel();
    }

    @NonNull
    @NotNull
    @Override
    public String getName() {
        return "MediaNotification";
    }

    @ReactMethod
    public void sendMediaNotification(ReadableMap param) {
        /*try {*/
        ShowNotificationTask task = new ShowNotificationTask();
        task.execute(param);
        /*    promise.resolve(null);
        } catch (Exception e) {
            promise.reject(e);
        }*/
    }


    private void createNotificationChannel() {
        // Create the NotificationChannel, but only on API 26+ because
        // the NotificationChannel class is new and not in the support library
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            CharSequence name = reactContext.getString(R.string.media_channel_name);
            String description = reactContext.getString(R.string.media_channel_description);
            int importance = NotificationManager.IMPORTANCE_DEFAULT;
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, importance);
            channel.setDescription(description);
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this
            NotificationManager notificationManager = reactContext.getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }


    private class ShowNotificationTask extends AsyncTask<ReadableMap, Integer, Bitmap> {
        private String name;
        private String artist;
        private Boolean isPlaying;

        private Bitmap getBitmap(String url) {
            Bitmap bm = null;
            try {
                URL iconUrl = new URL(url);
                URLConnection conn = iconUrl.openConnection();
                HttpURLConnection http = (HttpURLConnection) conn;
                int length = http.getContentLength();
                conn.connect();
                // 获得图像的字符流
                InputStream is = conn.getInputStream();
                BufferedInputStream bis = new BufferedInputStream(is, length);
                bm = BitmapFactory.decodeStream(bis);
                bis.close();
                is.close();// 关闭流
            } catch (Exception e) {
                e.printStackTrace();
            }
            return bm;
        }

        @Override
        protected Bitmap doInBackground(ReadableMap... parma) {
            this.isPlaying = parma[0].getBoolean("isPlaying");
            String picUrl = parma[0].getString("picUrl");
            this.artist = parma[0].getString("artist");
            this.name = parma[0].getString("name");
            return getBitmap(picUrl);
        }

        @Override
        protected void onPostExecute(Bitmap bitmap) {
            Notification notification;
            /*
             * setting the media session
             */
            MediaSession mediaSession = new MediaSession(reactContext, "sessionTag");
            mediaSession.setFlags(MediaSession.FLAG_HANDLES_MEDIA_BUTTONS | MediaSession.FLAG_HANDLES_TRANSPORT_CONTROLS);
            mediaSession.setActive(true);
            MediaSessionCompat mediaSessionCompat = android.support.v4.media.session.MediaSessionCompat.fromMediaSession(reactContext, mediaSession);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(reactContext, CHANNEL_ID)
                    .setVisibility(NotificationCompat.VISIBILITY_PUBLIC)
                    .setSmallIcon(R.drawable.ic_notifi)
                    .setContentTitle(name)
                    .setContentText(artist)
                    .setOngoing(true)
                    .setChannelId(CHANNEL_ID)
                    .addAction(R.drawable.ic_pre_button, "Pre", preIntent)
                    .addAction(isPlaying ? R.drawable.ic_pause_button : R.drawable.ic_play_button,
                            isPlaying ? "Pause" : "Play", isPlaying ? pauseIntent : playIntent)
                    .addAction(R.drawable.ic_next_button, "Next", nextIntent)
                    .setStyle(
                            new androidx.media.app.NotificationCompat.MediaStyle()
                                    .setMediaSession(mediaSessionCompat.getSessionToken())
                    );
            if (bitmap != null) {
                builder.setLargeIcon(bitmap);
            }
            notification = builder.build();
            notification.flags |= Notification.FLAG_NO_CLEAR;
            NotificationManagerCompat notificationManager = NotificationManagerCompat.from(reactContext);
            notificationManager.notify("media", NOTIFICATION_ID, notification);

        }
    }

    private static void sendEvent(ReactContext reactContext, String eventName) {
        reactContext.getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class).emit(eventName, null);
    }

    private static class ControlReceiver extends BroadcastReceiver {

        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            switch (action) {
                case ACTION_PLAY:
                    sendEvent(reactContext, "onPlayPressed");
                    break;
                case ACTION_PAUSE:
                    sendEvent(reactContext, "onPausePressed");
                    break;
                case ACTION_NEXT:
                    sendEvent(reactContext, "onNextPressed");
                    break;
                case ACTION_PRE:
                    sendEvent(reactContext, "onPrePressed");
                    break;
            }
        }
    }
}
