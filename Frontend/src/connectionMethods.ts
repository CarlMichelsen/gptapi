import type { HubConnection } from "@microsoft/signalr";
import type { MessageChunk } from "./types/dto/messageChunk";
import type { Message } from "./types/dto/message";
import type { FirstMessage } from "./types/dto/firstMessage";
import type { SendMessageRequest } from "./types/dto/sendMessageRequest";
import type { UpdateMessageId } from "./types/dto/updateMessageId";

type ConnectionMethod = (...args: any[]) => any;

export class ConnectionMethods {
    public static connection: HubConnection;

    public static sendMessage(request: SendMessageRequest) {
        this.connection.invoke("SendMessage", request);
    }
    // UpdateMessageId(UpdateMessageIdDto updateMessageId);
    public static updateMessageId(method: ((updateMessageId: UpdateMessageId) => void) | null)
    {
        this.registerMethod("updateMessageId", method);
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

    public static set receiveFirstMessage(method: ((firstMessage: FirstMessage) => any) | null) {
        this.registerMethod("receiveFirstMessage", method);
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