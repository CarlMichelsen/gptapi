import type { HubConnection } from "@microsoft/signalr";
import type { MessageChunk } from "./types/dto/messageChunk";
import type { Message } from "./types/dto/message";

type ConnectionMethod = (...args: any[]) => any;

export class ConnectionMethods {
    public static connection: HubConnection;

    public static sendMessage(content: string) {
        this.connection.invoke("SendMessage", content);
    }

    public static set disconnect(method: (() => void) | null) {
        this.registerMethod("disconnect", method);
    }

    public static set receiveMessageChunk(method: ((messageChunk: MessageChunk) => void) | null) {
        this.registerMethod("receiveMessageChunk", method);
    }

    public static set receiveMessage(method: ((message: Message) => any) | null) {
        this.registerMethod("receiveMessage", method);
    }

    private static registerMethod(methodName: string, method: ConnectionMethod | null) {
        if (method) {
            this.connection.on(methodName, method);
        } else {
            this.connection.off(methodName);
        }
    }
}