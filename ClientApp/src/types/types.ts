
export interface IrcMessage {
    connectionId: string;
    chatId: string;
    sender: string;
    message: string;
    timestamp: string;
}

// Functions

export interface CreateIrcConnectionFunction {
    connectionId: string;
}

export interface GetIrcCredentialsFunction {
    nick: string;
    password: string;
}

export interface GetSettingsFunction {
    apiToken: string;
    isUserHighlighted: boolean;
    language: string;
    alertOnMention: boolean;
    alertOnKeyword: boolean;
    highlightOnKeyword: boolean;
    keywordList: string[];
    showBeatmapBanner: boolean;
}
