package cz.datlowe.ehealth.encyclopedia.app.server.dao.domain.enc;

import java.util.List;

import com.hp.hpl.jena.rdf.model.Model;
import com.hp.hpl.jena.rdf.model.Resource;

import cz.datlowe.ehealth.encyclopedia.app.server.dao.ThingFactory;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.Thing;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.ATCConcept;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.DiseaseOrFinding;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.Ingredient;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.MechanismOfAction;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.MedicinalProduct;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.MedicinalProductGroup;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.Pharmacokinetics;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.PharmacologicalAction;
import cz.datlowe.ehealth.encyclopedia.app.shared.vo.domain.enc.PhysiologicEffect;

public class EncyclopediaThingFactory extends ThingFactory {
	
	private ATCConceptExtractor atcConceptExtractor = null;
	private DiseaseOrFindingExtractor diseaseExtractor = null;
	private IngredientExtractor ingredientExtractor = null;
	private MedicinalProductExtractor medicinalProductExtractor = null;
	private MedicinalProductGroupExtractor medicinalProductGroupExtractor = null;
	private PharmacologicalActionExtractor pharmacologicalActionExtractor = null;
	private MechanismOfActionExtractor mechanismOfActionExtractor = null;
	private PhysiologicEffectExtractor physiologicEffectExtractor = null;
	private PharmacokineticsExtractor pharmacokineticsExtractor = null;
	
	protected Thing constructThing(String typeURI, String resourceURI)	{
		if ( ATCConcept.typeURI.equals(typeURI) )	{
			return new ATCConcept(resourceURI);
		} else if ( Ingredient.typeURI.equals(typeURI) )	{
			return new Ingredient(resourceURI);
		} else if ( MedicinalProduct.typeURI.equals(typeURI) )	{
			return new MedicinalProduct(resourceURI);
		} else if ( MedicinalProductGroup.typeURI.equals(typeURI) )	{
			return new MedicinalProductGroup(resourceURI);
		} else if ( DiseaseOrFinding.typeURI.equals(typeURI) )	{
			return new DiseaseOrFinding(resourceURI);
		} else if ( PharmacologicalAction.typeURI.equals(typeURI) )	{
			return new PharmacologicalAction(resourceURI);
		} else if ( MechanismOfAction.typeURI.equals(typeURI) )	{
			return new MechanismOfAction(resourceURI);
		} else if ( PhysiologicEffect.typeURI.equals(typeURI) )	{
			return new PhysiologicEffect(resourceURI);
		} else if ( Pharmacokinetics.typeURI.equals(typeURI) )	{
			return new Pharmacokinetics(resourceURI);
		}
		
		throw new IllegalArgumentException("Unsupported type " + typeURI + " for constructing a thing from the Encyclopedia domain.");		
	}
	
	public Thing extractThingDetail(String typeURI, Resource resource, Model model, String requiredLang, String defaultLang) throws IllegalArgumentException	{
		Thing thing = null;
		
		try	{
			thing = createThing(typeURI, resource);
		} catch (IllegalArgumentException e)	{
			throw e;
		}
 
		if ( thing instanceof ATCConcept )	{
			atcConceptExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof DiseaseOrFinding )	{
			diseaseExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof Ingredient )	{
			ingredientExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof MedicinalProduct )	{
			medicinalProductExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof MedicinalProductGroup )	{
			medicinalProductGroupExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof PharmacologicalAction )	{
			pharmacologicalActionExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof MechanismOfAction )	{
			mechanismOfActionExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof PhysiologicEffect )	{
			physiologicEffectExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		} else if ( thing instanceof Pharmacokinetics )	{
			pharmacokineticsExtractor.extractThingDetail(thing, resource, model, requiredLang, defaultLang);
		}
		else {
			throw new IllegalArgumentException("Unsupported type " + typeURI + " for extracting a thing detail from the Encyclopedia domain.");
		}
		
		return thing;
	}
		
	public Thing extractThingInteractions(String typeURI, Resource resource, Model model, String requiredLang, String defaultLang) throws IllegalArgumentException	{
		Thing thing = null;
		
		try	{
			thing = createThing(typeURI, resource);
		} catch (IllegalArgumentException e)	{
			throw e;
		}
 
		if ( thing instanceof Ingredient )	{
			ingredientExtractor.extractThingInteractions(thing, resource, model, requiredLang, defaultLang);
		} else {
			throw new IllegalArgumentException("Unsupported type " + typeURI + " for extracting interactions of a thing from the Encyclopedia domain.");
		}
		
		return thing;
	}
	
	public Thing extractThingAlternatives(String typeURI, Resource resource, List<Thing> param, Model model, String requiredLang, String defaultLang) throws IllegalArgumentException	{
		Thing thing = null;
		
		try	{
			thing = createThing(typeURI, resource);
		} catch (IllegalArgumentException e)	{
			throw e;
		}
 
		if ( thing instanceof Ingredient )	{
			ingredientExtractor.extractThingAlternatives(thing, resource, param, model, requiredLang, defaultLang);
		} else {
			throw new IllegalArgumentException("Unsupported type " + typeURI + " for extracting interactions of a thing from the Encyclopedia domain.");
		}
		
		return thing;
	}
	
	public EncyclopediaThingFactory()	{
		super();
		
		extractor = new EncyclopediaThingExtractor(this);
		
		atcConceptExtractor = new ATCConceptExtractor(this);
		diseaseExtractor = new DiseaseOrFindingExtractor(this);
		ingredientExtractor = new IngredientExtractor(this);
		medicinalProductExtractor = new MedicinalProductExtractor(this);
		medicinalProductGroupExtractor = new MedicinalProductGroupExtractor(this);
		pharmacologicalActionExtractor = new PharmacologicalActionExtractor(this);
		mechanismOfActionExtractor = new MechanismOfActionExtractor(this);
		physiologicEffectExtractor = new PhysiologicEffectExtractor(this);
		pharmacokineticsExtractor = new PharmacokineticsExtractor(this);
	}
	
	public String getThingSearchSPARQLWhere(String[] prefixes, String requiredLang, String defaultLang)	{
		
		//We must start at 1 because the first prefix is already encoded in the query!
		String otherFilters = "";
		for (int i = 1; i < prefixes.length; i++) {
			otherFilters += "    FILTER (regex(?%s, \"" + prefixes[i] + "\", \"i\"))\n";
		}
		
		String query = 
				"CONSTRUCT {\n" +
				"  ?resource a ?type ;\n" +
				"            enc:title ?text ;\n" +
				"            enc:alternativeTitle ?altText .\n" +
				"} WHERE {\n" +
				"  {\n" +
				"    ?resource a enc:ATCConcept ;\n" +
				"              skos:prefLabel ?label ;\n" +
				"              skos:notation ?notation .\n" +
				"    ?label bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "label") +
				"    BIND (enc:ATCConcept AS ?type)\n" +
				"    BIND (strlang(bif:concat(?notation, \" \", ?label), lang(?label)) AS ?text)\n" +
				"  }\n" +
				"  UNION\n" +
				"  {\n" +
				"    { \n" +
				"      ?resource a enc:Ingredient ;\n" +
				"                enc:title ?text .\n" +
				"      ?text bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "text") +
				"    }\n" +
				"    UNION\n" +
				"    { \n" +
				"      ?resource a enc:Ingredient ;\n" +
				"                enc:title ?text ;\n" +
				"                dcterms:title ?altText .\n" +
				"      ?altText bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "altText") + 
				"    }\n" +			
				"    UNION\n" +
				"    { \n" +
				"      ?resource a enc:Ingredient ;\n" +
				"                dcterms:title ?text .\n" +
				"      ?text bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "text") + 
				"    }\n" +
				"    BIND (enc:Ingredient AS ?type)\n" +
				"  }\n" +
				"  UNION\n" +
				"  {\n" +
				"    ?resource a enc:MedicinalProduct ;\n" +
				"              dcterms:title ?title ;\n" +
				"              sukl:hasPackagingSize ?size .\n" +
				"    ?title bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "title") +
				"    BIND (enc:MedicinalProduct AS ?type)\n" +
				"    BIND (strlang(bif:concat(?title, \" (\", ?size, \")\"), lang(?title)) AS ?text)\n" +
				"  }\n" +
				"  UNION\n" +
				"  {\n" +
				"    ?resource a enc:MedicinalProductGroup ;\n" +
				"              dcterms:title ?text .\n" +
				"    ?text bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "text") +
				"    BIND (enc:MedicinalProductGroup AS ?type)\n" +
				"  }\n" +
				"  UNION\n" +
				"  {\n" +
				"    ?resource a enc:DiseaseOrFinding ;\n" +
				"              mesh:hasConcept ?concept .\n" +
				"    ?concept skos:prefLabel ?text .\n" +
				"    ?text bif:contains '\"" + prefixes[0] + "*\"' .\n" + String.format(otherFilters, "text") +
				"    BIND (enc:DiseaseOrFinding AS ?type)\n" +
				"  }\n" +
				"}";
		
		return query;
		
	}
	
	public String getThingDetailSPARQLQuery(String typeURI, String thingURI)	{
		String query = "";		
		
		if ( ATCConcept.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT {\n" +
					"  <" + thingURI + "> a enc:ATCConcept ;\n" +
					"    skos:prefLabel ?prefLabel ;\n" +
					"    skos:notation ?notation ;\n" +
					"    enc:description ?description ;\n" +
					"    skos:broaderTransitive ?broader ;\n" +
					"    skos:narrowerTransitive ?narrower ;\n" +
					"    enc:hasMedicinalProductGroup ?mpg .\n" +
					"  ?mpg a enc:MedicinalProductGroup ;\n" +
					"    enc:title ?mpgTitle ;\n" +
					"    enc:description ?mpgDescription ;\n" +
					"    enc:hasRouteOfAdministration ?mpgroaLabel ;\n" +
					"    enc:hasActiveIngredient ?mpgai .\n" +
					"  ?mpgai a enc:Ingredient ;\n" +
					"    enc:title ?mpgaiTitle ;\n" +
					"    enc:description ?mpgaiDescription ;\n" +
					"    enc:hasPregnancyCategory ?pc ;\n" +
					"    enc:hasPharmacologicalAction ?mpgaipa .\n" +
					"  ?mpgaipa a enc:PharmacologicalAction ;\n" +
					"    enc:title ?mpgaipaTitle ;\n" +
					"    enc:description ?mpgaipaDescription .\n" +
					"  ?narrower a enc:ATCConcept ;\n" +
					"    skos:prefLabel ?narrowerPrefLabel ;\n" +
					"    enc:description ?narrowerDescription ;\n" +
					"    skos:notation ?narrowerNotation .\n" +
					"  ?broader a enc:ATCConcept ;\n" +
					"    skos:prefLabel ?broaderPrefLabel ;\n" +
					"    enc:description ?broaderDescription ;\n" +
					"    skos:notation ?broaderNotation .\n" +
					"} WHERE {\n" +
					"  {\n" +
					"    <" + thingURI + "> a enc:ATCConcept ;\n" +
					"      skos:prefLabel ?prefLabel ;\n" +
					"      skos:notation ?notation .\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" + 
					"        enc:description ?description .\n" +
					"      }\n" +
					"  } UNION {\n" +
					"    ?mpg a enc:MedicinalProductGroup ;\n" +
					"      enc:hasATCConcept <" + thingURI + "> ;\n" +
					"      enc:title ?mpgTitle .\n" +
					"    OPTIONAL {\n" +
					"      {\n" +
					"        ?mpg enc:hasRouteOfAdministration/skos:prefLabel ?mpgroaLabel .\n" +
					"      } UNION {\n" +
					"        ?mpg enc:description ?mpgDescription .\n" +
					"      }\n" +
					"    }\n" +
					"    ?mpgai a enc:Ingredient ;\n" +
					"      enc:hasMedicinalProductGroup ?mpg ;\n" +
					"      enc:title ?mpgaiTitle .\n" +
					"    OPTIONAL {\n" +
					"      {\n" +
					"        ?mpgai enc:description ?mpgaiDescription .\n" +
					"      } UNION {\n" +
					"        ?mpgai enc:hasPharmacologicalAction ?mpgaipa .\n" +
					"        ?mpgaipa enc:title ?mpgaipaTitle .\n" +
					"        OPTIONAL {\n" +
					"          ?mpgaipa enc:description ?mpgaipaDescription .\n" +
					"        }\n" +
					"      } UNION {\n" +
					"        ?mpgai enc:hasPregnancyCategory ?pc .\n" +
					"      }\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    <" + thingURI + "> skos:narrowerTransitive+ ?narrower .\n" +
					"    {\n" +
					"      ?narrower\n" +
					"        skos:prefLabel ?narrowerPrefLabel ;\n" +
					"        skos:notation ?narrowerNotation .\n" +
					"    } UNION {\n" +
					"      ?narrower\n" + 
					"        enc:description ?narrowerDescription .\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    <" + thingURI + "> skos:broaderTransitive+ ?broader .\n" +
					"    {\n" +
					"      ?broader\n" +
					"        skos:prefLabel ?broaderPrefLabel ;\n" +
					"        skos:notation ?broaderNotation .\n" +
					"    } UNION {\n" +
					"      ?broader\n" + 
					"        enc:description ?broaderDescription .\n" +
					"    }\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT {
  <http://linked.opendata.cz/resource/ATC/M01AB05> a enc:ATCConcept ;
    skos:prefLabel ?prefLabel ;
    skos:notation ?notation ;
    skos:broaderTransitive ?broader ;
    skos:narrowerTransitive ?narrower ;
    enc:hasMedicinalProductGroup ?mpg .
  ?mpg a enc:MedicinalProductGroup ;
    enc:title ?mpgTitle ;
    enc:description ?mpgDescription ;
    enc:hasRouteOfAdministration ?mpgroaLabel ;
    enc:hasActiveIngredient ?mpgai .
  ?mpgai a enc:Ingredient ;
    enc:title ?mpgaiTitle ;
    enc:description ?mpgaiDescription ;
    enc:hasPregnancyCategory ?pc ;
    enc:hasPharmacologicalAction ?mpgaipa .
  ?mpgaipa a enc:PharmacologicalAction ;
    enc:title ?mpgaipaTitle ;
    enc:description ?mpgaipaDescription .
  ?narrower a enc:ATCConcept ;
    skos:prefLabel ?narrowerPrefLabel ;
    skos:notation ?narrowerNotation .
  ?broader a enc:ATCConcept ;
    skos:prefLabel ?broaderPrefLabel ;
    skos:notation ?broaderNotation .
} WHERE {
  {
    <http://linked.opendata.cz/resource/ATC/M01AB05> a enc:ATCConcept ;
      skos:prefLabel ?prefLabel ;
      skos:notation ?notation .
  } UNION {
    ?mpg a enc:MedicinalProductGroup ;
      enc:hasATCConcept <http://linked.opendata.cz/resource/ATC/M01AB05> ;
      enc:title ?mpgTitle .
    OPTIONAL {
      {
        ?mpg enc:hasRouteOfAdministration/skos:prefLabel ?mpgroaLabel .
      } UNION {
        ?mpg enc:description ?mpgDescription .
      }
    }

    ?mpgai a enc:Ingredient ;
      enc:hasMedicinalProductGroup ?mpg ;
      enc:title ?mpgaiTitle .
    OPTIONAL {
      {
        ?mpgai enc:description ?mpgaiDescription .
      } UNION {
        ?mpgai enc:hasPharmacologicalAction ?mpgaipa .
        ?mpgaipa enc:title ?mpgaipaTitle .
        OPTIONAL {
          ?mpgaipa enc:description ?mpgaipaDescription .
        }
      } UNION {
        ?mpgai enc:hasPregnancyCategory ?pc .
      }
    }
  } UNION {
    <http://linked.opendata.cz/resource/ATC/M01AB05> skos:narrowerTransitive+ ?narrower .
    ?narrower
      skos:prefLabel ?narrowerPrefLabel ;
      skos:notation ?narrowerNotation .
  } UNION {
    <http://linked.opendata.cz/resource/ATC/M01AB05> skos:broaderTransitive+ ?broader .
    ?broader
      skos:prefLabel ?broaderPrefLabel ;
      skos:notation ?broaderNotation .
  }
}
 */
		} else if ( DiseaseOrFinding.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT\n" +
					"{\n" +
					"  <" + thingURI + "> a enc:DiseaseOrFinding ;\n" +
					"    enc:title ?title ;\n" +
					"    enc:description ?description ;\n" +    
					"    enc:mayBeTreatedBy ?mt ;\n" +
					"    enc:mayBePreventedBy ?mp ;\n" +
					"    enc:contraindicatedWith ?ci ;\n" +
					"    skos:narrower ?narrower ;\n" +
					"    skos:broader ?broader .\n" +
					"  ?mt a enc:Ingredient ;\n" +
					"    enc:title ?mtTitle ;\n" +
					"    enc:description ?mtDescription .\n" +
					"  ?mp a enc:Ingredient ;\n" +
					"    enc:title ?mpTitle ;\n" +
					"    enc:description ?mpDescription .\n" +
					"  ?ci a enc:Ingredient ;\n" +
					"    enc:title ?ciTitle ;\n" +
					"    enc:description ?ciDescription .\n" +
					"  ?narrower\n" +
					"    enc:title ?narrowerPrefLabel .\n" +
					"  ?broader\n" +
					"    enc:title ?broaderPrefLabel  .\n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:title ?title .\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:description ?description .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    ?mt\n" +
					"      enc:mayTreat <" + thingURI + "> ;\n" +
					"      enc:title ?mtTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?mt\n" +
					"        enc:description ?mtDescription .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    ?mp\n" +
					"      enc:mayPrevent <" + thingURI + "> ;\n" +
					"      enc:title ?mpTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?mp\n" +
					"        enc:description ?mpDescription .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    ?ci\n" +
					"      enc:contraindicatedWith <" + thingURI + "> ;\n" +
					"      enc:title ?ciTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?ci\n" +
					"        enc:description ?ciDescription .\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    <" + thingURI + ">\n" +
					"      ^skos:broader ?narrower .\n" +
					"    ?narrower\n" +
					"      enc:title ?narrowerPrefLabel .\n" +
					"  } UNION {\n" +
					"    <" + thingURI + ">\n" +
					"      skos:broader ?broader .\n" +
					"    ?broader\n" +
					"      enc:title ?broaderPrefLabel  .\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278> a enc:DiseaseOrFinding ;
    enc:title ?title ;
    enc:description ?description ;    
    enc:mayBeTreatedBy ?mt ;
    enc:mayBePreventedBy ?mp ;
    enc:contraindicatedWith ?ci ;
    skos:narrower ?narrower ;
    skos:broader ?broader .
  ?mt a enc:Ingredient ;
    enc:title ?mtTitle ;
    enc:description ?mtDescription .
  ?mp a enc:Ingredient ;
    enc:title ?mpTitle ;
    enc:description ?mpDescription .
  ?ci a enc:Ingredient ;
    enc:title ?ciTitle ;
    enc:description ?ciDescription .
  ?narrower
    enc:title ?narrowerPrefLabel .
  ?broader
    enc:title ?broaderPrefLabel  .
}
WHERE
{
  {
    <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278>
      enc:title ?title .
    OPTIONAL {
      <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278>
        enc:description ?description .
    }
  }
  UNION
  {
    ?mt
      enc:mayTreat <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278> ;
      enc:title ?mtTitle .
    OPTIONAL {
      ?mt
        enc:description ?mtDescription .
    }
  }
  UNION
  {
    ?mp
      enc:mayPrevent <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278> ;
      enc:title ?mpTitle .
    OPTIONAL {
      ?mp
        enc:description ?mpDescription .
    }
  }
  UNION
  {
    ?ci
      enc:contraindicatedWith <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278> ;
      enc:title ?ciTitle .
    OPTIONAL {
      ?ci
        enc:description ?ciDescription .
    }
  } UNION {
    <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278>
      ^skos:broader ?narrower .
    ?narrower
      enc:title ?narrowerPrefLabel .
  } UNION {
    <http://linked.opendata.cz/resource/ndfrt/disease/N0000002278>
      skos:broader ?broader .
    ?broader
      enc:title ?broaderPrefLabel  .
  }
}
 */
			
		} else if ( Ingredient.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT\n" +
					"{\n" +
					"  <" + thingURI + "> a enc:Ingredient ;\n" +
					"    enc:title ?title ;\n" +
					"    enc:description ?description ;\n" +
					"    enc:indication ?indication ;\n" +
					"    enc:hasPharmacologicalAction ?pa ;\n" +
					"    enc:hasMechanismOfAction ?moa ;\n" +
					"    enc:hasPhysiologicEffect ?pe ;\n" +
					"    enc:hasPharmacokinetics ?pk ;\n" +
					"    enc:hasPregnancyCategory ?pc ;\n" +
					"    enc:mayTreat ?mt ;\n" +
					"    enc:mayPrevent ?mp ;\n" +
					"    enc:contraindicatedWith ?ci ;\n" +
					"    enc:hasMedicinalProductGroup ?mpg .\n" +
					"  ?pa a enc:PharmacologicalAction ;\n" +
					"    enc:title ?paTitle ;\n" +
					"    enc:description ?paDescription .\n" +
					"  ?moa a enc:MechanismOfAction ;\n" +
					"    enc:title ?moaTitle .\n" +
					"  ?pe a enc:PhysiologicEffect ;\n" +
					"    enc:title ?peTitle .\n" +
					"  ?pk a enc:Pharmacokinetics ;\n" +
					"    enc:title ?pkTitle .\n" +
					"  ?mt a enc:DiseaseOrFinding ;\n" +
					"    enc:title ?mtTitle ;\n" +
					"    enc:description ?mtDescription .\n" +
					"  ?mp a enc:DiseaseOrFinding ;\n" +
					"    enc:title ?mpTitle ;\n" +
					"    enc:description ?mpDescription .\n" +
					"  ?ci a enc:DiseaseOrFinding ;\n" +
					"    enc:title ?ciTitle ;\n" +
					"    enc:description ?ciDescription .\n" +
					"  ?mpg a enc:MedicinalProductGroup ;\n" +
					"    enc:title ?mpgTitle ;\n" +
					"    enc:description ?mpgDescription ;\n" +
					"    enc:hasRouteOfAdministration ?mpgRoALabel ;\n" +
					"    enc:hasDosageForm ?mpgDFLabel ;\n" +
					"    enc:hasATCConcept ?mpgATC .\n" +
					"  ?mpgATC a enc:ATCConcept ;\n" +
					"    skos:prefLabel ?mpgATCLabel ;\n" +
					"    skos:notation ?mpgATCNotation .\n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:title ?title .\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:description ?description .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:indication ?indication .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:hasPharmacologicalAction ?pa .\n" +
					"      ?pa enc:title ?paTitle .\n" +
					"      OPTIONAL {\n" +
					"        ?pa enc:description ?paDescription .\n" +
					"      }\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:hasMechanismOfAction ?moa .\n" +
					"      ?moa enc:title ?moaTitle .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:hasPhysiologicEffect ?pe .\n" +
					"      ?pe enc:title ?peTitle .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:hasPharmacokinetics ?pk .\n" +
					"      ?pk enc:title ?pkTitle .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:hasPregnancyCategory ?pc .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:hasMedicinalProductGroup ?mpg .\n" +
					"    ?mpg enc:title ?mpgTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?mpg enc:description ?mpgDescription .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mpg enc:hasRouteOfAdministration ?mpgRoA .\n" +
					"      ?mpgRoA skos:prefLabel ?mpgRoALabel .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mpg enc:hasDosageForm ?mpgDF .\n" +
					"      ?mpgDF skos:prefLabel ?mpgDFLabel .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mpg enc:hasATCConcept ?mpgATC .\n" +
					"      ?mpgATC skos:prefLabel ?mpgATCLabel ;\n" +
					"              skos:notation ?mpgATCNotation .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:mayTreat ?mt .\n" +
					"    ?mt enc:title ?mtTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?mt enc:description ?mtDescription .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:mayPrevent ?mp .\n" +
					"    ?mp enc:title ?mpTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?mp enc:description ?mpDescription .\n" +
					"    }\n" +
					"  }\n" +
					"  UNION\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:contraindicatedWith ?ci .\n" +
					"    ?ci enc:title ?ciTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?ci enc:description ?ciDescription .\n" +
					"    }\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626> a enc:Ingredient ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasPharmacologicalAction ?pa ;
    enc:hasMechanismOfAction ?moa ;
    enc:hasPhysiologicEffect ?pe ;
    enc:hasPharmacokinetics ?pk ;
    enc:hasPregnancyCategory ?pc ;
    enc:mayTreat ?mt ;
    enc:mayPrevent ?mp ;
    enc:contraindicatedWith ?ci ;
    enc:hasMedicinalProductGroup ?mpg .
  ?pa a enc:PharmacologicalAction ;
    enc:title ?paTitle ;
    enc:description ?paDescription .
  ?moa a enc:MechanismOfAction ;
    enc:title ?moaTitle .
  ?pe a enc:PhysiologicEffect ;
    enc:title ?peTitle .
  ?pk a enc:Pharmacokinetics ;
    enc:title ?pkTitle .
  ?mt a enc:DiseaseOrFinding ;
    enc:title ?mtTitle ;
    enc:description ?mtDescription .
  ?mp a enc:DiseaseOrFinding ;
    enc:title ?mpTitle ;
    enc:description ?mpDescription .
  ?ci a enc:DiseaseOrFinding ;
    enc:title ?ciTitle ;
    enc:description ?ciDescription .
  ?mpg a enc:MedicinalProductGroup ;
    enc:title ?mpgTitle ;
    enc:description ?mpgDescription ;
    enc:hasRouteOfAdministration ?mpgRoALabel ;
    enc:hasDosageForm ?mpgDFLabel ;
    enc:hasATCConcept ?mpgATC .
  ?mpgATC a enc:ATCConcept ;
    skos:prefLabel ?mpgATCLabel ;
    skos:notation ?mpgATCNotation .
}
WHERE
{
  {
    <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
      enc:title ?title .
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:description ?description .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:indication ?indication .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:hasPharmacologicalAction ?pa .
      ?pa enc:title ?paTitle .
      OPTIONAL {
	    ?pa enc:description ?paDescription .
      }
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:hasMechanismOfAction ?moa .
      ?moa enc:title ?moaTitle .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:hasPhysiologicEffect ?pe .
      ?pe enc:title ?peTitle .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:hasPhysiologicEffect ?pk .
      ?pk enc:title ?pkTitle .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:hasPregnancyCategory ?pc .
    }
  }
  UNION
  {
    <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
      enc:hasMedicinalProductGroup ?mpg .
    ?mpg enc:title ?mpgTitle .
    OPTIONAL {
      ?mpg enc:description ?mpgDescription .
    }
    OPTIONAL {
      ?mpg enc:hasRouteOfAdministration ?mpgRoA .
      ?mpgRoA skos:prefLabel ?mpgRoALabel .
    }
    OPTIONAL {
      ?mpg enc:hasDosageForm ?mpgDF .
      ?mpgDF skos:prefLabel ?mpgDFLabel .
    }
    OPTIONAL {
      ?mpg enc:hasATCConcept ?mpgATC .
      ?mpgATC skos:prefLabel ?mpgATCLabel ;
              skos:notation ?mpgATCNotation .
    }
  }
  UNION
  {
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:mayTreat ?mt .
      ?mt enc:title ?mtTitle .
      OPTIONAL {
        ?mt enc:description ?mtDescription .
      }
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:mayPrevent ?mp .
      ?mp enc:title ?mpTitle .
      OPTIONAL {
        ?mp enc:description ?mpDescription .
      }
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0013626>
        enc:contraindicatedWith ?ci .
      ?ci enc:title ?ciTitle .
      OPTIONAL {
        ?ci enc:description ?ciDescription .
      }
    }
  }
}
 */
		} 
		else if ( MedicinalProduct.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT\n" +
					"{\n" +
					"  <" + thingURI + "> a enc:MedicinalProduct ;\n" +
					"    enc:title ?title ;\n" +
					"    skos:notation ?notation ;\n" +
					"    enc:hasRouteOfAdministration ?roaLabel ;\n" +
					"    enc:hasStrength ?strength ;\n" +
					"    enc:hasPackagingSize ?size ;\n" +
					"    enc:hasActiveIngredient ?ai ;\n" +
					"    enc:hasATCConcept ?atc ;\n" +
					"    enc:hasMedicinalProductGroup ?mpg ;\n" +
					"    enc:hasSPCDocument ?spcDocument ;\n" +
					"    enc:hasIndication ?indicationText ;\n" +
					"    enc:hasTextChunkInSPCInteractionSection ?textChunk .\n" +
					"  ?textChunk a sdo:TextChunk ;\n" +
					"    sdo:hasText ?textChunkText ;\n" +
					"    sdo:hasStartPointer ?textChunkStartPointer ;\n" +
					"    sdo:hasEndPointer ?textChunkEndPointer ;\n" +
					"    sdo:hasSentence ?sentence ;\n" +
					"    dcterms:title ?topicTitle .\n" +
					"  ?sentence a sdo:Sentence ;\n" +
					"    sdo:hasText ?sentenceText ;\n" +
					"    sdo:hasStartPointer ?sentenceStartPointer ;\n" +
					"    sdo:hasEndPointer ?sentenceEndPointer .\n" +
					"  ?mpg a enc:MedicinalProductGroup ;\n" +
					"    enc:title ?mpgTitle .\n" +
					"  ?ai a enc:ActiveIngredient ;\n" +
					"    enc:title ?aiTitle ;\n" +
					"    skos:prefLabel ?aiPrefLabel ;\n" +
					"    enc:hasPharmacologicalAction ?aiPa .\n" +
					"  ?aiPa a enc:PharmacologicalAction ;\n" +
					"    enc:title ?aiPaTitle .\n" +
					"  ?atc a enc:ATCConcept ;\n" +
					"    dcterms:title ?atcTitle ;\n" +
					"    skos:notation ?atcNotation ;\n" +
					"    skos:broaderTransitive ?atc1 .\n" +
					"  ?atc1 a enc:ATCConcept ;\n" +
					"    dcterms:title ?atc1Title ;\n" +
					"    skos:notation ?atc1Notation .\n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"  {\n" +  
					"    <" + thingURI + ">\n" +
					"      dcterms:title ?title ;\n" +
					"      skos:notation ?notation ;\n" +
					"      enc:hasATCConcept ?atc ;\n" +
					"      enc:hasMedicinalProductGroup ?mpg .\n" +
					"    ?mpg\n" +
					"      enc:title ?mpgTitle .\n" +
					"    OPTIONAL {\n" +
					"      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasRouteOfAdministration ?roa .\n" +
					"      ?roa skos:prefLabel ?roaLabel .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasStrength ?strength .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasPackagingSize ?size .\n" +
					"    }\n" +
					"    ?atc dcterms:title ?atcTitle ;\n" +
					"         skos:notation ?atcNotation ;\n" +
					"         skos:broaderTransitive+ ?atc1 .\n" +
					"    ?atc1 dcterms:title ?atc1Title ;\n" +
					"          skos:notation ?atc1Notation .\n" +
					"  } UNION {\n" +
					"    <" + thingURI + ">\n" +
					"      sukl:hasActiveIngredient/^owl:sameAs ?ai .\n" +
					"    ?ai enc:title ?aiTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?ai mesh:hasConcept ?aiConcept .\n" +
					"      ?aiConcept mesh:scopeNote ?aiNote ;\n" +
					"                 skos:prefLabel ?aiPrefLabel .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?ai enc:hasPharmacologicalAction ?aiPa .\n" +
					"      ?aiPa enc:title ?aiPaTitle .\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    <" + thingURI + "> spc:hasSPC/spc:hasDocument ?spcDocument .\n" +
					"    {\n" +
					"      ?spcDocument sdo:hasSection ?indicationSection .\n" +
					"      ?indicationSection sdo:hasOrderNumber ?indicationOrderNumber .\n" +
					"       FILTER (?indicationOrderNumber = \"004.001\")\n" +
					"      ?indicationSection sdo:hasText ?indicationText .\n" +
					"    } UNION {\n" +
					"      ?spcDocument sdo:hasSection ?interactionSection .\n" +
					"      ?interactionSection sdo:hasOrderNumber ?interactionOrderNumber .\n" +
					"      FILTER (?interactionOrderNumber = \"004.005\")\n" +
					"      ?interactionSection sdo:hasParagraph/sdo:hasSentence ?sentence .\n" +
					"      ?sentence sdo:hasTextChunk ?textChunk ;\n" +
					"        sdo:hasText ?sentenceText ;\n" +
					"        sdo:hasStartPointer ?sentenceStartPointer ;\n" +
					"        sdo:hasEndPointer ?sentenceEndPointer .\n" +
					"      ?textChunk sdo:hasAnnotation/sao:hasTopic ?topic ;\n" +
					"        sdo:hasText ?textChunkText ;\n" +
					"        sdo:hasStartPointer ?textChunkStartPointer ;\n" +
					"        sdo:hasEndPointer ?textChunkEndPointer .\n" +
					"      FILTER (?topic != spc:interactionEventDescription)\n" +
					"      ?topic a ?topicType ;\n" +
					"        enc:title ?topicTitle .\n" +
					"    }\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX sao: <http://salt.semanticauthoring.org/ontologies/sao#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/sukl/medicinal-product/0031109> a enc:MedicinalProduct ;
    enc:title ?title ;
    skos:notation ?notation ;
    enc:hasRouteOfAdministration ?roaLabel ;
    enc:hasStrength ?strength ;
    enc:hasPackagingSize ?size ;
    enc:hasActiveIngredient ?ai ;
    enc:hasATCConcept ?atc ;
    enc:hasMedicinalProductGroup ?mpg ;
    enc:hasSPCDocument ?spcDocument ;
    enc:hasIndication ?indicationText ;
    enc:hasTextChunkInSPCInteractionSection ?textChunk .
  ?textChunk a sdo:TextChunk ;
    sdo:hasText ?textChunkText ;
    sdo:hasStartPointer ?textChunkStartPointer ;
    sdo:hasEndPointer ?textChunkEndPointer ;
    sdo:hasSentence ?sentence ;
    dcterms:title ?topicTitle .
  ?sentence a sdo:Sentence ;
    sdo:hasText ?sentenceText ;
    sdo:hasStartPointer ?sentenceStartPointer ;
    sdo:hasEndPointer ?sentenceEndPointer .
  ?mpg a enc:MedicinalProductGroup ;
    enc:title ?mpgTitle .
  ?ai a enc:ActiveIngredient ;
    enc:title ?aiTitle ;
    skos:prefLabel ?aiPrefLabel ;
    enc:hasPharmacologicalAction ?aiPa .
  ?aiPa a enc:PharmacologicalAction ;
    enc:title ?aiPaTitle .
  ?atc a enc:ATCConcept ;
    dcterms:title ?atcTitle ;
    skos:notation ?atcNotation ;
    skos:broaderTransitive ?atc1 .
  ?atc1 a enc:ATCConcept ;
    dcterms:title ?atc1Title ;
    skos:notation ?atc1Notation .
}
WHERE
{
  {  
    <http://linked.opendata.cz/resource/sukl/medicinal-product/0031109>
      dcterms:title ?title ;
      skos:notation ?notation ;
      enc:hasATCConcept ?atc ;
      enc:hasMedicinalProductGroup ?mpg .
    ?mpg
      enc:title ?mpgTitle .
    OPTIONAL {
      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasRouteOfAdministration ?roa .
      ?roa skos:prefLabel ?roaLabel .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasStrength ?strength .
    }
    OPTIONAL {
      <http://linked.opendata.cz/resource/sukl/medicinal-product/0157986> sukl:hasPackagingSize ?size .
    }
    ?atc dcterms:title ?atcTitle ;
         skos:notation ?atcNotation ;
         skos:broaderTransitive+ ?atc1 .
    ?atc1 dcterms:title ?atc1Title ;
          skos:notation ?atc1Notation .
  } UNION {
    <http://linked.opendata.cz/resource/sukl/medicinal-product/0031109>
      sukl:hasActiveIngredient/^owl:sameAs ?ai .
    ?ai enc:title ?aiTitle .
    OPTIONAL {
      ?ai mesh:hasConcept ?aiConcept .
      ?aiConcept mesh:scopeNote ?aiNote ;
                 skos:prefLabel ?aiPrefLabel .
    }
    OPTIONAL {
      ?ai enc:hasPharmacologicalAction ?aiPa .
      ?aiPa enc:title ?aiPaTitle .
    }
  } UNION {
    <http://linked.opendata.cz/resource/sukl/medicinal-product/0031109> spc:hasSPC/spc:hasDocument ?spcDocument .
    {
      ?spcDocument sdo:hasSection ?indicationSection .
      ?indicationSection sdo:hasOrderNumber ?indicationOrderNumber .
       FILTER (?indicationOrderNumber = "004.001")
      ?indicationSection sdo:hasText ?indicationText .
    } UNION {
      ?spcDocument sdo:hasSection ?interactionSection .
      ?interactionSection sdo:hasOrderNumber ?interactionOrderNumber .
      FILTER (?interactionOrderNumber = "004.005")
      ?interactionSection sdo:hasParagraph/sdo:hasSentence ?sentence .
      ?sentence sdo:hasTextChunk ?textChunk ;
        sdo:hasText ?sentenceText ;
        sdo:hasStartPointer ?sentenceStartPointer ;
        sdo:hasEndPointer ?sentenceEndPointer .
      ?textChunk sdo:hasAnnotation/sao:hasTopic ?topic ;
        sdo:hasText ?textChunkText ;
        sdo:hasStartPointer ?textChunkStartPointer ;
        sdo:hasEndPointer ?textChunkEndPointer .
      FILTER (?topic != spc:interactionEventDescription)
      ?topic a ?topicType ;
        enc:title ?topicTitle .
    }
  }
}
 */
		} else if ( MedicinalProductGroup.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT\n" +
					"{\n" +
					"  <" + thingURI + "> a enc:MedicinalProductGroup ;\n" +
					"    enc:title ?title ;\n" +
					"    enc:description ?description ;\n" +
					"    enc:hasActiveIngredient ?ai ;\n" +
					"    enc:hasATCConcept ?atc ;\n" +
					"    enc:hasMedicinalProduct ?mp ;\n" +
				    "    enc:hasPregnancyCategory ?aiPregnancyCategory .\n" +
					"  ?mp a enc:MedicinalProduct ;\n" +
					"    enc:title ?mpTitle ;\n" +
					"    enc:description ?mpDescription ;\n" +
					"    skos:notation ?notation ;\n" +
					"    enc:hasRouteOfAdministration ?roaLabel ;\n" +
					"    enc:hasStrength ?strength ;\n" +
					"    enc:hasPackagingSize ?size .\n" +
					"  ?ai a enc:Ingredient ;\n" +
					"    enc:title ?aiTitle ;\n" +
					"    enc:description ?aiDescription ;\n" +
					"    enc:hasPharmacologicalAction ?aiPa .\n" +
					"  ?aiPa a enc:PharmacologicalAction ;\n" +
					"    enc:title ?aiPaTitle ;\n" +
					"    enc:description ?aiPaDescription .\n" +
					"  ?atc a enc:ATCConcept ;\n" +
					"    dcterms:title ?atcTitle ;\n" +
					"    skos:notation ?atcNotation ;\n" +
					"    skos:broaderTransitive ?atc1 .\n" +
					"  ?atc1 a enc:ATCConcept ;\n" +
					"    dcterms:title ?atc1Title ;\n" +
					"    skos:notation ?atc1Notation .\n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:title ?title .\n" +
					"     OPTIONAL {\n" +
					"       <" + thingURI + ">\n" +
					"        enc:description ?description .\n" +
					"     }\n" +
					"  } UNION {\n" +
					"    ?mp a enc:MedicinalProduct ;\n" +
					"      enc:hasMedicinalProductGroup <" + thingURI + "> ;\n" +
					"      dcterms:title ?mpTitle ;\n" +
					"      skos:notation ?notation .\n" +
					"    OPTIONAL {\n" +
					"      ?mp\n" +
					"        enc:description ?mpDescription .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mp\n" +
					"        sukl:hasRouteOfAdministration/skos:prefLabel ?roaLabel .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mp\n" +
					"        sukl:hasStrength ?strength .\n" +
					"    }\n" +
					"    OPTIONAL {\n" +
					"      ?mp\n" +
					"        sukl:hasPackagingSize ?size .\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:hasATCConcept ?atc .\n" +
					"    ?atc\n" +
					"      dcterms:title ?atcTitle ;\n" +
					"      skos:notation ?atcNotation ;\n" +
					"      skos:broaderTransitive+ ?atc1 .\n" +
					"    ?atc1\n" +
					"      dcterms:title ?atc1Title ;\n" +
					"      skos:notation ?atc1Notation .\n" +
					"  } UNION {\n" +
					"    ?ai\n" +
					"      enc:hasMedicinalProductGroup <" + thingURI + "> ;\n" +
					"      enc:title ?aiTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?ai\n" +
					"        enc:description ?aiDescription .\n" +
					"    }\n" +
					"    OPTIONAL {" +
					"      ?ai" +
					"        enc:hasPregnancyCategory ?aiPregnancyCategory ." +
					"    }" +
					"    OPTIONAL {\n" +
					"      ?ai\n" +
					"        enc:hasPharmacologicalAction ?aiPa .\n" +
					"      ?aiPa\n" +
					"        enc:title ?aiPaTitle .\n" +
					"      OPTIONAL {\n" +
					"        ?aiPa\n" +
					"          enc:description ?aiPaDescription .\n" +
					"      }\n" +
					"    }\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT> a enc:MedicinalProductGroup ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasActiveIngredient ?ai ;
    enc:hasATCConcept ?atc ;
    enc:hasMedicinalProduct ?mp ;
    enc:hasPregnancyCategory ?aiPregnancyCategory .
  ?mp a enc:MedicinalProduct ;
    enc:title ?mpTitle ;
    enc:description ?mpDescription ;
    skos:notation ?notation ;
    enc:hasRouteOfAdministration ?roaLabel ;
    enc:hasStrength ?strength ;
    enc:hasPackagingSize ?size .
  ?ai a enc:Ingredient ;
    enc:title ?aiTitle ;
    enc:description ?aiDescription ;
    enc:hasPharmacologicalAction ?aiPa .
  ?aiPa a enc:PharmacologicalAction ;
    enc:title ?aiPaTitle ;
    enc:description ?aiPaDescription .
  ?atc a enc:ATCConcept ;
    dcterms:title ?atcTitle ;
    skos:notation ?atcNotation ;
    skos:broaderTransitive ?atc1 .
  ?atc1 a enc:ATCConcept ;
    dcterms:title ?atc1Title ;
    skos:notation ?atc1Notation .
}
WHERE
{
  {
    <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT>
      enc:title ?title .
    OPTIONAL {
      <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT>
        enc:description ?description .
    }
  } UNION {
    ?mp a enc:MedicinalProduct ;
      enc:hasMedicinalProductGroup <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT> ;
      dcterms:title ?mpTitle ;
      skos:notation ?notation .
    OPTIONAL {
      ?mp
        enc:description ?mpDescription .
    }
    OPTIONAL {
      ?mp
        sukl:hasRouteOfAdministration/skos:prefLabel ?roaLabel .
    }
    OPTIONAL {
      ?mp
        sukl:hasStrength ?strength .
    }
    OPTIONAL {
      ?mp
        sukl:hasPackagingSize ?size .
    }
  } UNION {
    <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT>
      enc:hasATCConcept ?atc .
    ?atc
      dcterms:title ?atcTitle ;
      skos:notation ?atcNotation ;
      skos:broaderTransitive+ ?atc1 .
    ?atc1
      dcterms:title ?atc1Title ;
      skos:notation ?atc1Notation .
  } UNION {
    ?ai
      enc:hasMedicinalProductGroup <http://linked.opendata.cz/resource/sukl/medicinal-product-group/DOPEGYT> ;
      enc:title ?aiTitle .
    OPTIONAL {
      ?ai
        enc:description ?aiDescription .
    }
    OPTIONAL {
      ?ai
        enc:hasPregnancyCategory ?aiPregnancyCategory .
    }
    OPTIONAL {
      ?ai
        enc:hasPharmacologicalAction ?aiPa .
      ?aiPa
        enc:title ?aiPaTitle .
      OPTIONAL {
        ?aiPa
          enc:description ?aiPaDescription .
      }
    }
  }
}
 */
		} else if ( PharmacologicalAction.typeURI.equals(typeURI) )	{
			query =
    			"CONSTRUCT\n" +
    			"{\n" +
    			"  <" + thingURI + "> a enc:PharmacologicalAction ;\n" +
    			"    enc:title ?title ;\n" +
    			"    enc:description ?description ;\n" +
    			"    enc:hasRelatedActiveIngredient ?ai .\n" +
    			"  ?ai a enc:Ingredient ;\n" +
    			"    enc:title ?aiTitle ;\n" +
    			"    enc:description ?aiDescription .\n" +
    			"}\n" +
    			"WHERE\n" +
    			"{\n" +
    			"  <" + thingURI + ">\n" +
    			"    enc:title ?title .\n" +
    			"  OPTIONAL {\n" +
    			"    <" + thingURI + ">\n" +
    			"      enc:description ?description .\n" +
    			"  }\n" +
    			"  ?ai\n" +
    			"    enc:hasPharmacologicalAction <" + thingURI + "> ;\n" +
    			"    enc:title ?aiTitle .\n" +
    			"  OPTIONAL {\n" +
    			"    ?ai\n" +
    			"      enc:description ?aiDescription .\n" +
    			"  }\n" +
    			"}";
			
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038> a enc:PharmacologicalAction ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasRelatedActiveIngredient ?ai .
  ?ai a enc:Ingredient ;
    enc:title ?aiTitle ;
    enc:description ?aiDescription .
}
WHERE
{
  <http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038>
    enc:title ?title .
  OPTIONAL {
    <http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038>
      enc:description ?description .
  }
  ?ai
    enc:hasPharmacologicalAction <http://linked.opendata.cz/resource/drug-encyclopedia/pharmacological-action/M0028038> ;
    enc:title ?aiTitle .
  OPTIONAL {
    ?ai
      enc:description ?aiDescription .
  }
}
 */
		} 
		
		else if ( MechanismOfAction.typeURI.equals(typeURI) ) {
			query =
	    			"CONSTRUCT\n" +
	    			"{\n" +
	    			"  <" + thingURI + "> a enc:MechanismOfAction ;\n" +
	    			"    enc:title ?title ;\n" +
	    			"    enc:description ?description ;\n" +
	    			"    enc:hasRelatedActiveIngredient ?ai .\n" +
	    			"  ?ai a enc:Ingredient ;\n" +
	    			"    enc:title ?aiTitle ;\n" +
	    			"    enc:description ?aiDescription .\n" +
	    			"}\n" +
	    			"WHERE\n" +
	    			"{\n" +
	    			"  <" + thingURI + ">\n" +
	    			"    enc:title ?title .\n" +
	    			"  OPTIONAL {\n" +
	    			"    <" + thingURI + ">\n" +
	    			"      enc:description ?description .\n" +
	    			"  }\n" +
	    			"  ?ai\n" +
	    			"    enc:hasMechanismOfAction <" + thingURI + "> ;\n" +
	    			"    enc:title ?aiTitle .\n" +
	    			"  OPTIONAL {\n" +
	    			"    ?ai\n" +
	    			"      enc:description ?aiDescription .\n" +
	    			"  }\n" +
	    			"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/ndfrt/mechanism-of-action/N0000009957> a enc:MechanismOfAction ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasRelatedActiveIngredient ?ai .
  ?ai a enc:Ingredient ;
    enc:title ?aiTitle ;
    enc:description ?aiDescription .
}
WHERE
{
  <http://linked.opendata.cz/resource/ndfrt/mechanism-of-action/N0000009957>
    enc:title ?title .
  OPTIONAL {
    <http://linked.opendata.cz/resource/ndfrt/mechanism-of-action/N0000009957>
      enc:description ?description .
  }
  ?ai
    enc:hasMechanismOfAction <http://linked.opendata.cz/resource/ndfrt/mechanism-of-action/N0000009957> ;
    enc:title ?aiTitle .
  OPTIONAL {
    ?ai
      enc:description ?aiDescription .
  }
}
 */
		}
		
		else if ( PhysiologicEffect.typeURI.equals(typeURI) ) {
			query =
	    			"CONSTRUCT\n" +
	    			"{\n" +
	    			"  <" + thingURI + "> a enc:PhysiologicEffect ;\n" +
	    			"    enc:title ?title ;\n" +
	    			"    enc:description ?description ;\n" +
	    			"    enc:hasRelatedActiveIngredient ?ai .\n" +
	    			"  ?ai a enc:Ingredient ;\n" +
	    			"    enc:title ?aiTitle ;\n" +
	    			"    enc:description ?aiDescription .\n" +
	    			"}\n" +
	    			"WHERE\n" +
	    			"{\n" +
	    			"  <" + thingURI + ">\n" +
	    			"    enc:title ?title .\n" +
	    			"  OPTIONAL {\n" +
	    			"    <" + thingURI + ">\n" +
	    			"      enc:description ?description .\n" +
	    			"  }\n" +
	    			"  ?ai\n" +
	    			"    enc:hasPhysiologicEffect <" + thingURI + "> ;\n" +
	    			"    enc:title ?aiTitle .\n" +
	    			"  OPTIONAL {\n" +
	    			"    ?ai\n" +
	    			"      enc:description ?aiDescription .\n" +
	    			"  }\n" +
	    			"}";

/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000008836> a enc:PhysiologicEffect ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasRelatedActiveIngredient ?ai .
  ?ai a enc:Ingredient ;
    enc:title ?aiTitle ;
    enc:description ?aiDescription .
}
WHERE
{
  <http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000008836>
    enc:title ?title .
  OPTIONAL {
    <http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000008836>
      enc:description ?description .
  }
  ?ai
    enc:hasPhysiologicEffect <http://linked.opendata.cz/resource/ndfrt/physiologic-effect/N0000008836> ;
    enc:title ?aiTitle .
  OPTIONAL {
    ?ai
      enc:description ?aiDescription .
  }
}
 */
		}
		
		else if ( Pharmacokinetics.typeURI.equals(typeURI) ) {
			query =
	    			"CONSTRUCT\n" +
	    			"{\n" +
	    			"  <" + thingURI + "> a enc:Pharmacokinetics ;\n" +
	    			"    enc:title ?title ;\n" +
	    			"    enc:description ?description ;\n" +
	    			"    enc:hasRelatedActiveIngredient ?ai .\n" +
	    			"  ?ai a enc:Ingredient ;\n" +
	    			"    enc:title ?aiTitle ;\n" +
	    			"    enc:description ?aiDescription .\n" +
	    			"}\n" +
	    			"WHERE\n" +
	    			"{\n" +
	    			"  <" + thingURI + ">\n" +
	    			"    enc:title ?title .\n" +
	    			"  OPTIONAL {\n" +
	    			"    <" + thingURI + ">\n" +
	    			"      enc:description ?description .\n" +
	    			"  }\n" +
	    			"  ?ai\n" +
	    			"    enc:hasPharmacokinetics <" + thingURI + "> ;\n" +
	    			"    enc:title ?aiTitle .\n" +
	    			"  OPTIONAL {\n" +
	    			"    ?ai\n" +
	    			"      enc:description ?aiDescription .\n" +
	    			"  }\n" +
	    			"}";

/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000026> a enc:Pharmacokinetics ;
    enc:title ?title ;
    enc:description ?description ;
    enc:hasRelatedActiveIngredient ?ai .
  ?ai a enc:Ingredient ;
    enc:title ?aiTitle ;
    enc:description ?aiDescription .
}
WHERE
{
  <http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000026>
    enc:title ?title .
  OPTIONAL {
    <http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000026>
      enc:description ?description .
  }
  ?ai
    enc:hasPharmacokinetics <http://linked.opendata.cz/resource/ndfrt/pharmacokinetics/N0000000026> ;
    enc:title ?aiTitle .
  OPTIONAL {
    ?ai
      enc:description ?aiDescription .
  }
}
 */
		}
		return query;
	}
	
	public String getThingInteractionsSPARQLQuery(String typeURI, String thingURI)	{
		String query = "";		
		
		if ( Ingredient.typeURI.equals(typeURI) )	{
			query =
					"CONSTRUCT\n" +
					"{\n" +
					"  ?interaction a enc:Interaction ;\n" +
					"    enc:ingredient <" + thingURI + "> ;\n" +
					"    enc:ingredient ?interacting ;\n" +
					"    enc:description ?interactionDescription ;\n" +
					"    enc:severity ?interactionSeverity ;\n" +
					"    enc:source ?interactionSource .\n" +
					"  <" + thingURI + "> a enc:Ingredient ;\n" +
					"    enc:title ?title ;\n" +
					"    enc:description ?description .\n" +
					"  ?interacting a enc:Ingredient ;\n" +
					"    enc:title ?interactingTitle ;\n" +
					"    enc:description ?interactingDescription .\n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"  {\n" +
					"    <" + thingURI + ">\n" +
					"      enc:title ?title .\n" +
					"    OPTIONAL {\n" +
					"      <" + thingURI + ">\n" +
					"        enc:description ?description .\n" +
					"    }\n" +
					"  } UNION {\n" +
					"    ?interaction a enc:Interaction ;\n" +
					"      dcterms:source ?interactionSource ;\n" +
					"      enc:ingredient <" + thingURI + "> ;\n" +
					"      enc:ingredient ?interacting .\n" +
					"    FILTER (?interacting != <" + thingURI + ">)\n" +
					"    ?interacting\n" +
					"      enc:title ?interactingTitle .\n" +
					"    OPTIONAL {\n" +
					"      ?interacting\n" +
					"        enc:description ?interactingDescription .\n" +
					"    }\n" +
					"    OPTIONAL\n" +
					"    {\n" +
					"      ?interaction enc:severity ?interactionSeverity .\n" +
					"    }\n" +
					"    OPTIONAL\n" +
					"    {\n" +
					"      ?interaction enc:hasText ?interactionDescription .\n" +
					"    }\n" +
					"  }\n" +
					"}";
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  ?interaction a enc:Interaction ;
    enc:ingredient <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965> ;
    enc:ingredient ?interacting ;
    enc:description ?interactionDescription ;
    enc:severity ?interactionSeverity ;
    enc:source ?interactionSource .
  <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965> a enc:Ingredient ;
    enc:title ?title ;
    enc:description ?description .
  ?interacting a enc:Ingredient ;
    enc:title ?interactingTitle ;
    enc:description ?interactingDescription .
}
WHERE
{
  {
    <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965>
      enc:title ?title .
    OPTIONAL {
      <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965>
        enc:description ?description .
    }
  } UNION {
    ?interaction a enc:Interaction ;
      dcterms:source ?interactionSource ;
      enc:ingredient <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965> ;
      enc:ingredient ?interacting .
    FILTER (?interacting != <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965>)
    ?interacting
      enc:title ?interactingTitle .
    OPTIONAL {
      ?interacting
        enc:description ?interactingDescription .
    }
    OPTIONAL
    {
      ?interaction enc:severity ?interactionSeverity .
    }
    OPTIONAL
    {
      ?interaction enc:hasText ?interactionDescription .
    }
  }
}
 */
		}
	
		return query;
	}
	
	public String getThingAlternativesSPARQLQuery(String typeURI, String thingURI, List<Thing> param)	{
		String query = "";		
		
		if ( Ingredient.typeURI.equals(typeURI) )	{
			query = 
					"CONSTRUCT\n" +
					"{\n" +
					" <" + thingURI + "> a enc:Ingredient ;\n" +
					"   enc:title ?title ; \n" +
					"   enc:hasAlternative ?alternative .\n" +
					" ?alternative a enc:Ingredient ; \n" +
					"   enc:title ?titleAlternative ; \n" +
					"   enc:hasPharmacologicalAction ?action . \n" +
					"}\n" +
					"WHERE\n" +
					"{\n" +
					"    <" + thingURI + ">\n" +
					"      enc:title ?title ;\n" +
					"      enc:hasPharmacologicalAction ?action . \n" +
					"	 ?alternative a enc:Ingredient ; \n" +
					"      enc:hasPharmacologicalAction ?action ;\n" +
					"      enc:title ?titleAlternative .\n" +
					"}";
	    }
		
/*
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX sukl: <http://linked.opendata.cz/ontology/sukl/>
PREFIX mesh: <http://linked.opendata.cz/ontology/mesh/>
PREFIX ndf: <http://linked.opendata.cz/ontology/ndfrt/>
PREFIX dbank: <http://linked.opendata.cz/ontology/drugbank/>
PREFIX nci: <http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#>
PREFIX spl: <http://linked.opendata.cz/ontology/fda/spl/>
PREFIX spc: <http://linked.opendata.cz/ontology/spc/>
PREFIX atc: <http://linked.opendata.cz/ontology/sukl/>
PREFIX sdo: <http://salt.semanticauthoring.org/ontologies/sdo#>
PREFIX enc: <http://linked.opendata.cz/ontology/drug-encyclopedia/>
PREFIX adms: <http://www.w3.org/ns/adms#>

CONSTRUCT
{
  <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965> a enc:Ingredient ;
    enc:title ?title ; 
    enc:hasPharmacologicalAction ?action ;
    enc:hasAlternative ?alternative .
  ?alternative a enc:Ingredient ;
    enc:hasPharmacologicalAction ?action .

}
WHERE
{
  <http://linked.opendata.cz/resource/drug-encyclopedia/ingredient/M0010965> enc:title ?title;
    enc:hasPharmacologicalAction ?action .
  ?alternative a enc:Ingredient ;
    enc:hasPharmacologicalAction ?action .
}
 */
		return query;
	}
	
	
		
}
