import React, { Component } from 'react';

import { Form, FormGroup, ControlLabel, FormControl, Button, Checkbox, Row, Col} from 'react-bootstrap';
import avatar from '../../../styles/images/avatar1.jpg';
import TablePA from './TablePA';



export default class PerformaceAppraisal extends Component {
    constructor(props) {
        super(props);
    }    
    render() {
        return(
                <div className="col-sm-12">
                    <Form inline className="form-filter">
                        <FormGroup>
                            <ControlLabel>Evaluation Period</ControlLabel>       
                            <FormControl componentClass="select" className="employee-field">
                                <option>Second Half 2018</option>
                                <option>First Half 2018</option>
                                <option>Second Half 2017</option>
                                <option>First Half 2017</option>
                            </FormControl>     
                        </FormGroup>   
                        <FormGroup>
                            <Button bsStyle="primary" className="btn-primary">Search</Button>
                        </FormGroup>
                    </Form>
                    <TablePA />
                </div>
        );
    }
}


