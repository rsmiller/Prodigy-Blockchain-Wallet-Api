# Prodigy Blockchain Wallet Api
This project contains the node code for Prodigy Blockchain, a certificate document-based blockchain. This app will allow you to generate a document blockchain that is hosted on your server/msa or in the cloud. This will also allow you to create the first initial wallet for the blockchain if you intend to do an import of data.

Please read the [documentation regarding Prodigy Blockchain](https://prodigychain.bit.ai/rdc/j9xA8uLDLVOgIZtL) to understand the use cases.

## Use Case for Prodigy Blockchain
The use case for Prodigy is storing simple documents like manufacturing product certificates, certificates of conformance, certificates of test, and more. These documents are encrypted and stored in a block on a chain that is accessible via a unique identifier like a serial number, document number, or order number. 

This product is not made for worldwide general use. This is made for manufacturing companies to allow their customers access to those certificates in a unique and marketable way.

Mining is simply an audit mechanism and doesn't need to exist outside of auditing in this use case. However, for marketability, mining allows employees of a company to earn tokens/coins that can be spent for days off, company merchandise, or whatever in a company store. How this is done is in the [block explorer project](https://github.com/rsmiller/Prodigy-Blockchain-Explorer) and the [wallet api project](https://github.com/rsmiller/Prodigy-Blockchain-Wallet-Api).

## Wallet API Installation
This is a .NET 6 application so ideally, you will need a Visual Studio product to modify and compile.

The BusinessLayer project contains the sql script to create the database and tables. Out of the box this is tailored to MySQL. Additional small modifications will be needed to use MSSQL.

Once the database is created you will need to modify appsettings.json and include information specific to your organization. It is not recommended to use default data.

The test application will demonstrate how the wallets are created and stored.

## Support
I created Prodigy Blockchain as a proof of concept and something to cure boredom. I am not really supporting this or these Prodigy Blockchain projects actively at this time because I have a full-time job, so feel free to fork and do as you wish. If you have questions or want help implementing this for your company you can reach me here: www.linkedin.com/in/more-guids or ryan@prodigyblockchain.com. My one-time fee is between $2000 - $5000 depending on your cert process, how they are generated, how many you intend to import (7+ years?), database setup, on-prem/cloud, and other business factors. If you need product support beyond that we can negotiate and help one another in the venture.

## License
Do as ye will but if ye a be making booty off this code a kickback of a new phone, camping equipment, computer, couch, or car would be appreciated, yyar.
