export class CommonHelper {
    private static readonly INVALID_CHARACTERS: Array<string> = ['\\', '/', ':', '*', '?', '<', '>', '|', '"'];

    public static hasInvalidCharacters(name: string): boolean {
        if (!name) {
            return true;
        }

        for (const invalidChar of this.INVALID_CHARACTERS) {
            if (name.includes(invalidChar)) {
                return true;
            }
        }

        return false;
    }
}
