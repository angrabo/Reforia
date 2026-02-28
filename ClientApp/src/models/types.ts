import { ErrorCode } from "./ErrorCode";

export interface WebRequest {
    requestId: string;
    functionName: string;
    body?: string;
}

export interface WebResponse<TResult = any> {
    requestId: string;
    statusCode: number;
    body: TResult | null;
    errorCode: ErrorCode;
    details: string;
}
