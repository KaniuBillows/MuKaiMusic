export function onError(error) {
    console.error(error);
}
export function onResult(result): boolean {
    if (result && result.code && result.code != 200 && result.error) {
       //alert(result.error);
    }
    return !(result && result.code && result.code != 200 && result.error);
}