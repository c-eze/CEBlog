let index = 0;

function AddTag() {
    //Get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry"); 

    //Lets use the new search function to help detect an error state
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        //Trigger my sweet alert for whatever condition is contained in the searchResult var
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        });
    }
    else {
        //Create a new Select option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }
        
    //Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: "<span class='font-weight-bolder'>CHOOSE A TAG BEFORE DELETING</span>"
        });
        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
        }
        index--;
    }
}

$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})

//Look for the tagValues variable to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        //Load up or Replace the options that we have 
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}

//The Search function will detect either an empty or a duplicate Tag
//and return an error string if an error is detected 
function search(str) {
    if (str == "") {
        return 'Empty tags are not permitted';
    }

    var tagsEl = document.getElementById('TagList');
    if (tagsEl) {
        let options = tagsEl.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {
                return `The Tag #${str} was detected as a duplicate and not permitted`
            }
        }
    }
}

function AddRelatedPosts() {
    //Get a reference to the PostTitle select element
    var relatedPost = document.getElementById("PostTitle"); 
    var objPosts = document.getElementById("PostList");

    //Lets use the new search function to help detect an error state
    let searchResult = searchTitle(relatedPost.options[relatedPost.selectedIndex].value);
    if (searchResult != null) {
        //Trigger my sweet alert for whatever condition is contained in the searchResult var
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        })
    }
    else 
    {
        if (objPosts.options.length < 4) {
            //Create a new Select option
            let newOption = new Option(relatedPost.options[relatedPost.selectedIndex].text, relatedPost.options[relatedPost.selectedIndex].text);
            objPosts.options[index++] = newOption;
        }
    }

    return true;
}

//The SearchTitle function will detect either an empty or a duplicate Title name
//and return an error string if an error is detected 
function searchTitle(str) {
    if (str == "") {
        return 'Empty title names are not permitted';
    }

    var postsEl = document.getElementById('PostList');
    if (postsEl) {
        let options = postsEl.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {
                return `The title name #${str} was detected as a duplicate and not permitted`
            }
        }
    }
}

function RemoveRelatedPosts() {
    let postCount = 1;
    let postList = document.getElementById("PostList");
    if (!postList) return false;

    if (postList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: "<span class='font-weight-bolder'>CHOOSE A TITLE BEFORE DELETING</span>"
        });
        return true;
    }

    while (postCount > 0) {
        if (postList.selectedIndex >= 0) {
            postList.options[postList.selectedIndex] = null;
            --postCount
        }
        else {
            postCount = 0;
        }
        index--;
    }
}

//Select all of the post entries so that they transfer into http post
//when Submit button is pressed
$("form").on("submit", function () {
    $("#PostList option").prop("selected", "selected");
})


//Look for the postValues variable to see if it has data
if (postValues != '') {
    let postArray = postValues.split(",");
    for (let loop = 0; loop < postArray.length; loop++) {
        //Load up or Replace the options that we have 
        ReplacePost(postArray[loop], loop);
        index++;
    }
}

function ReplacePost(title, index) {
    let newOption = new Option(title, title);
    document.getElementById("PostList").options[index] = newOption;
} 

const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm btn-block btn-outline-dark'
    },
    imageUrl: '/img/oops.png',
    timer: 3000,
    buttonsStyling: false
})