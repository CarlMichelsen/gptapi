<script lang="ts">

</script>

<div class="grid grid-cols-[200px_minmax(0,1fr)]">
    <div>
        <slot name="sidebar"></slot>
    </div>

    <div>
        <slot name="conversation"></slot>
    </div>
</div>