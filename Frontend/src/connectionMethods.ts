import type { HubConnection } from "@microsoft/signalr";
import type { MessageChunk } from "./types/dto/conversation/messageChunk";
import type { ReceiveMessage } from "./types/dto/conversation/ReceiveMessage";
import type { SendMessageRequest } from "./types/dto/conversation/sendMessageRequest";
import type { CarlGptError } from "./types/dto/error";

type ConnectionMethod = (...args: any[]) => any;

export class ConnectionMethods {
    public static connection: HubConnection;

    public static sendMessage(request: SendMessageRequest) {
        this.connection.invoke("SendMessage", request);
    }

    public static cancelMessage(streamIdentifier: string) {
        this.connection.invoke("CancelMessage", streamIdentifier);
    }

    public static set disconnect(method: (() => void) | null) {
        this.registerMethod("disconnect", method);
    }

    public static set receiveMessageChunk(method: ((messageChunk: MessageChunk) => void) | null) {
        this.registerMethod("receiveMessageChunk", method);
    }
    
    public static set receiveMessage(method: ((message: ReceiveMessage) => any) | null) {
        this.registerMethod("receiveMessage", method);
    }

    public static set error(method: ((error: CarlGptError) => any) | null) {
        this.registerMethod("error", method);
    }

    public static set assignSummaryToConversation(method: ((conversationId: string, summary: string) => any) | null) {
        this.registerMethod("assignSummaryToConversation", method);
    }

    private static registerMethod(methodName: string, method: ConnectionMethod | null) {
        if (method) {
            this.connection.on(methodName, method);
        } else {
            this.connection.off(methodName);
        }
    }
}