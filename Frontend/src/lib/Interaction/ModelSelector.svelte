<script lang="ts">
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
    import type { LanguageModel } from "../../types/store/applicationStore";

    export let languageModel: LanguageModel | null;
    export let selectModel: (model: AvailableModel) =>  void;
</script>

<div>
    <p>SELECTOR</p>
</div>