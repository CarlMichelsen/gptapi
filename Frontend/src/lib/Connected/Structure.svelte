<script lang="ts">

</script>

<div class="grid grid-rows-[50px_minmax(0,1fr)] md:grid-cols-[300px_minmax(0,1fr)]">
    <div>
        <slot name="sidebar"></slot>
    </div>

    <div>
        <slot name="conversation"></slot>
    </div>
</div>