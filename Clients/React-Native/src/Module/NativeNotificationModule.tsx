import { NativeEventEmitter, NativeModules } from "react-native";

export default NativeModules.MediaNotification as MediaNotification;

interface MediaNotification {
    sendMediaNotification(message: NotificationMessage): void;
}

interface NotificationMessage {
    name: string,
    artist: string,
    picUrl: string,
    isPlaying: boolean
}
const eventEmiter = new NativeEventEmitter(NativeModules.MediaNotification);
export function subscribeMediaEvent(eventName: "onPlayPressed" | "onPausePressed" | "onNextPressed" | "onPrePressed",
    listener: (...args: any[]) => any) {
    eventEmiter.addListener(eventName, listener);
}
export function unSubscribeMediaEvent(eventName: "onPlayPressed" | "onPausePressed" | "onNextPressed" | "onPrePressed",
    listener: (...args: any[]) => any) {
    eventEmiter.removeListener(eventName, listener);
}
