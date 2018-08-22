import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Select from 'react-select';
import { Tabs, Tab} from 'react-bootstrap';
import PerformanceCheckIn  from './PCI';
import PerformaceAppraisal  from './PA';




const options = [
    {value: "All", label: "all"},
    {value: "Raymond", label: "Raymond"},
    {value: "Tim", label: "Tim"}, 
    {value: "Hunter", label: "Hunter"}, 
    {value: "Oliver", label: "Oliver"},
    {value: "Troy", label: "Troy"}, 
    {value: "Larry", label: "Larry"}
];

export default class SelfEvaluation extends Component {
    constructor(props) {
        super(props);
    }    
    render() {
        return(
            <div className="row">
                <Breadcrumb>
                    <li className="current">Self Evaluation</li>
                </Breadcrumb> 
                <div className="col-sm-12">
                    <Tabs id="tab-self">
                        <Tab title="Performance Checked In" eventKey={1}>
                            <PerformanceCheckIn />
                        </Tab>
                        <Tab title="Performace Appraisal" eventKey={2}>
                            <PerformaceAppraisal />
                        </Tab>
                    </Tabs>  
                </div>              
            </div>
        ) ;
                 
    }
}

ReactDOM.render(<SelfEvaluation />, document.getElementById("self-evaluation-wrapper"));

