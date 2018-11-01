function r2d_hi(){
    console.log("HI");
}

let myFirstPromise = new Promise((resolve, reject) => {
    // Chiamiamo resolve(...) quando viene eseguito correttamente, e reject(...) quando fallisce.
    // In questo esempio viene utilizzato setTimeout(...) per simulare un'operazione asincrona.
    // Nella realtà probabilmente utilizzerai qualcosa del tipo XHR o HTML5 API.
    setTimeout(function () {
        resolve("Success!"); // È andato tutto perfettamente!
    }, 250);
});

myFirstPromise.then((successMessage) => {
    // successMessage viene passato alla funzione resolve(...) .
    // Non deve essere necessariamente una stringa, ma nel caso sia solo un messaggio probabilmemte lo sarà.
    console.log("Yay! " + successMessage);
});