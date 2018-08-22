import React, { Component } from 'react';

import { FormGroup, ControlLabel, FormControl, Button, Checkbox, Row, Col} from 'react-bootstrap';
import avatar from '../../../styles/images/avatar1.jpg';
import TablePA from './TablePA';



export default class PerformaceAppraisal extends Component {
    constructor(props) {
        super(props);
    }    
    render() {
        return(
                <div className="col-sm-12">
                    <form horizontal="true" className="form-filter">
                        <FormGroup>
                            <ControlLabel>Employee <span>*</span></ControlLabel>       
                            <FormControl componentClass="select" className="employee-field">
                                <option>All</option>
                                <option>Tim</option>
                                <option>Elton</option>
                                <option>Quinn</option>
                            </FormControl>     
                        </FormGroup>   
                        <FormGroup>
                            <Button bsStyle="primary" className="btn-primary">Search</Button>
                        </FormGroup>
                    </form>
                    <TablePA />
                </div>
        );
    }
}


