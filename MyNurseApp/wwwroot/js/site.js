// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function removeManipulation(id) {
    if (confirm("Are you sure you want to remove this manipulation?")) {
        fetch('/Manipulations/RemoveManipulation', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: id })
        }).then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert("Failed to remove manipulation.");
            }
        });
    }
}
