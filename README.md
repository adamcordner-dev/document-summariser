# DocumentSummariser

A RESTful ASP.NET Core Web API that uses the Anthropic Claude API to summarise documents and extract action points from text input.

Built as a portfolio project to demonstrate LLM API integration, clean architecture, dependency injection, and unit testing in a .NET Core context.

![Build and Test](https://github.com/adamcordner-dev/document-summariser/actions/workflows/dotnet.yml/badge.svg)

---

## Project Summary

Users submit text content to one of two endpoints. The API sends the content to Claude with a tailored system prompt and returns either a structured summary with key points, or a numbered list of decisions and action items extracted from the document.

This has practical relevance in document-heavy industries such as legal services, where quickly extracting decisions and actions from meeting minutes, contracts, or case notes is a common workflow.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| API Framework | ASP.NET Core Web API (.NET 8) |
| LLM Integration | Anthropic Claude API via [Anthropic.SDK](https://github.com/tghamm/Anthropic.SDK) |
| Testing | xUnit, Moq |
| Documentation | Swagger / OpenAPI |

---

## Architecture

The project follows a clean, layered architecture with clear separation of concerns:

```
DocumentSummariser/
├── Controllers/
│   └── SummariseController.cs       # HTTP routing and validation only
├── Models/
│   ├── SummariseRequest.cs          # Request model with validation attributes
│   └── SummariseResponse.cs         # Response model
├── Services/
│   ├── IAnthropicMessageClient.cs   # Abstraction over Anthropic API calls
│   ├── AnthropicMessageClient.cs    # Anthropic SDK integration
│   ├── ISummariseService.cs         # Summarisation service interface
│   └── SummariseService.cs          # Prompt logic and orchestration
DocumentSummariser.Tests/
├── SummariseControllerTests.cs      # Controller unit tests
└── SummariseServiceTests.cs         # Service unit tests
```

### Design decisions

- **Controller is thin** -> handles HTTP concerns only (routing, validation, response codes), delegates all logic to the service layer
- **`IAnthropicMessageClient` interface** -> abstracts the Anthropic SDK behind an interface, enabling the service layer to be unit tested without hitting the real API
- **`SummariseService`** -> owns prompt logic only; the two endpoints share the same underlying API call with different system prompts
- **Dependency injection** -> all dependencies registered in `Program.cs` and injected via constructors throughout
- **Defensive validation** -> two-layer input validation: `[Required]` attribute catches missing fields before the controller is reached; `IsNullOrWhiteSpace` catches empty or whitespace strings
- **Constants over magic strings** -> prompts, error messages, and repeated strings extracted as `private const` throughout

---

## Endpoints

### `POST /api/summarise`
Accepts a text input and returns a structured summary with key points.

**Request:**
```json
{
  "content": "Your document text here..."
}
```

**Response:**
```json
{
  "summary": "## Summary\n...\n\n## Key Points\n- ..."
}
```

---

### `POST /api/summarise/actions`
Accepts a text input and returns a numbered list of decisions made and action items identified in the document.

**Request:**
```json
{
  "content": "Your document text here..."
}
```

**Response:**
```json
{
  "summary": "**Decisions Made:**\n1. ...\n\n**Action Items:**\n2. ..."
}
```

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- An [Anthropic API key](https://console.anthropic.com) - you will need to create an account and add credits (minimum $5). Each request costs a fraction of a cent.

### Setup

```bash
# Clone the repo
git clone https://github.com/adamcordner-dev/document-summariser.git
cd document-summariser
```

Create `appsettings.Development.json` in the `DocumentSummariser` project folder (this file is excluded from source control):

```json
{
  "Anthropic": {
    "ApiKey": "your-api-key-here"
  }
}
```

### Run

```bash
cd DocumentSummariser
dotnet run
```

Swagger UI will be available at `https://localhost:{port}/swagger`.

### Run Tests

```bash
cd DocumentSummariser.Tests
dotnet test
```

---

## Example

The following example uses publicly available sample meeting minutes from Eastern Connecticut State University ([source](https://www.easternct.edu/student-activities/_documents/SampleMinutes-1.pdf)). This document was not created by the project author and is used here solely for demonstration purposes.

> **Note on input formatting:** The API currently accepts plain JSON strings. Multi-line documents should have line breaks removed before submission. See [Known Limitations](#known-limitations) below.

### Input (truncated)

```
Skateboarding Club 3.21.07 Shafer Hall Room 221. Attendance: President KayTe Bettencourt, Vice-President Dennis Mayes, Secretary Sarah Tynan, Treasurer Carrie Spaulding... The meeting was called to order at 6:02pm by President Bettencourt. Motion #9: To accept the minutes from the meeting on September 12, 2006. 7 in favor/0 opposed/1 abstention. Motion Carries...
```

### `POST /api/summarise` response (truncated)

```json
{
  "summary": "## Skateboarding Club Meeting Summary\n**Date:** March 21, 2007 | **Location:** Shafer Hall, Room 221\n\nThis meeting covered DJ booking for the upcoming SkateaPalooza event, financial updates, trip planning for a New York excursion, and announcements regarding upcoming executive board elections.\n\n## Key Points\n- **DJ Sk8 hired** for SkateaPalooza on May 18, 2007 for $600\n- **Account Balances:** Budget: $2,564.33 | Fundraising: $1,547.89\n- **NY Trip (April 28):** Hotel $1,010.00, $20 parking, $1,000 for meals approved\n- **Executive Board nominations** in two weeks..."
}
```

### `POST /api/summarise/actions` response (truncated)

```json
{
  "summary": "**Decisions Made:**\n1. Minutes from September 12, 2006 accepted.\n2. $600 approved for DJ Bryan Caplette for SkateaPalooza.\n3. $1,010.00 Purchase Order approved for Best Western hotel.\n4. $20.00 cash advance approved for parking in NY.\n5. $1,000.00 approved for meals ($50/person) for NY trip.\n\n**Action Items:**\n6. Submit finalized Constitution to Valerie Nettleton.\n7. C. Spaulding to submit Room Reservation Form for Webb Lawn by April 20th.\n8. Follow up on outstanding invoice from Boards 'R Us.\n9. Executive Board nominations in two weeks, elections the following week."
}
```

---

## Known Limitations

### Input format - newlines in JSON
The API currently accepts text as a JSON string. Raw multi-line text (e.g. copied directly from a document) may contain literal newline characters which are invalid in JSON strings and will return a 400 error. The workaround is to remove line breaks before submitting.

**Planned improvement:** Accept file uploads (`multipart/form-data`) so documents can be submitted directly without manual formatting.

### Temperature
The model temperature is currently fixed at `1.0` (default). For summarisation tasks, a lower value (e.g. `0.3`) may produce more consistent and deterministic output. This is a candidate for a configurable parameter in a future update.

---

## Full Example Output

### `POST /api/summarise` - full response

```json
{
  "summary": "## Skateboarding Club Meeting Summary\n**Date:** March 21, 2007 | **Location:** Shafer Hall, Room 221\n**Meeting called to order:** 6:02 PM | **Adjourned:** 6:45 PM\n\nThis meeting covered DJ booking for the upcoming SkateaPalooza event, financial updates, trip planning for a New York excursion, and announcements regarding upcoming executive board elections.\n\n---\n\n## Key Points\n\n- **Minutes** from the September 12, 2006 meeting were accepted (Motion #9)\n- **Constitution** has been finalized and will be submitted to advisor Valerie Nettleton\n- **First aid kits** will be distributed on April 12 at Winthrop Hall and other residence halls; Kim Cardinal offered to drive\n- **DJ Sk8 hired** for SkateaPalooza on May 18, 2007 (8PM–12AM) at Mystic Marriott for **$600** (Motion #10)\n- **Account Balances:** Budget (BAMJ08): $2,564.33 | Fundraising (BAMK08): $1,547.89\n- **Skateboard Workshop** event registration approved; Room Reservation Form to be submitted for Webb Lawn on April 20\n- **NY Trip (April 28):** Hotel approved for **$1,010.00** (Motion #11), **$20 parking** cash advance approved (Motion #12), and **$1,000 for meals** ($50/person for 20 attendees) approved from fundraising account (Motion #13)\n- **Executive Board nominations** in two weeks; elections the following week\n- **Announcements:** West Indian Society Patty Sale on Monday; President's Inauguration on April 13"
}
```

### `POST /api/summarise/actions` - full response

```json
{
  "summary": "Here is a concise numbered list of action points and decisions identified in the document:\n\n**Decisions Made:**\n1. Minutes from September 12, 2006 meeting were accepted (Motion #9).\n2. Approved payment of $600 to DJ Bryan Caplette (DJ Sk8) for SkateaPalooza on May 18, 2007 at Mystic Marriott, 8pm–12am (Motion #10).\n3. Approved $1,010.00 Purchase Order to Best Western for hotel accommodations for the NY trip on April 28th (Motion #11).\n4. Approved $20.00 cash advance to KayTe Bettencourt for parking in NY (Motion #12).\n5. Approved $1,000.00 cash advance ($50 per person) to Valerie Nettleton for meals for 20 attendees on the NY trip (Motion #13).\n\n**Action Items:**\n6. Submit finalized Constitution to Valerie Nettleton.\n7. Distribute first aid kits on April 12, 2006 at 5pm in Winthrop Hall and other Residence Halls — Kim Cardinal to drive the van.\n8. C. Spaulding to submit Room Reservation Form to Mark Massinda to book Webb Lawn for April 20th Skateboard Workshop.\n9. Vote on funding for the Skateboard Workshop event at the next meeting.\n10. Follow up on outstanding invoice from Boards 'R Us.\n11. Members interested in Executive Board positions should contact current position holders regarding responsibilities; nominations in two weeks, elections the following week."
}
```

---

## AI-Assisted Development

AI tools (GitHub Copilot and Claude) were used during development for SDK usage guidance and boilerplate generation. All architecture decisions, design patterns, validation logic, and test coverage were manually reviewed and implemented by the developer.

---

## Disclaimer

This project uses the Anthropic Claude API. Usage of the API is subject to [Anthropic's usage policies](https://www.anthropic.com/legal/usage-policy).

The sample meeting minutes used in the example section are publicly available from Eastern Connecticut State University and are used here for demonstration purposes only. They are not affiliated with this project.
