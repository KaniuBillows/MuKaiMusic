export function onError(error) {
    console.error(error);
}
export function onResult(result) {
    if (result.code && result.code != 200 && result.error) {
        console.error(result.error);
    }
}