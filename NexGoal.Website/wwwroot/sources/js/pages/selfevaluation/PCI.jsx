import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Select from 'react-select';
import { Form, FormGroup, ControlLabel, FormControl, Button, Row, Col} from 'react-bootstrap';
import avatar from '../../../styles/images/avatar1.jpg';



const options = [
    {value: "All", label: "all"},
    {value: "Raymond", label: "Raymond"},
    {value: "Tim", label: "Tim"}, 
    {value: "Hunter", label: "Hunter"}, 
    {value: "Oliver", label: "Oliver"},
    {value: "Troy", label: "Troy"}, 
    {value: "Larry", label: "Larry"}
];

export default class PerformanceCheckIn extends Component {
    constructor(props) {
        super(props);
    }    
    render() {
        const avataruser = `/dist/${avatar}`;
        return(
                <div className="col-sm-12">
                    <Form inline className="form-filter">
                        <div className="row-filter">
                            <FormGroup>
                                <ControlLabel>From Date</ControlLabel>       
                                <FormControl componentClass="select" className="employee-field">
                                    <option>All</option>
                                    <option>Tim</option>
                                    <option>Elton</option>
                                    <option>Quinn</option>
                                </FormControl>     
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>To Date</ControlLabel>
                                <FormControl componentClass="select" placeholder="Select Grade" className="grade-field">
                                    <option>Failed (F)</option>
                                    <option>Poor (P)</option>
                                    <option>Unsatisfactory (U)</option>
                                    <option>Unsatisfactory +(U+)</option>
                                </FormControl>
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>Creator</ControlLabel>
                                <FormControl componentClass="select" placeholder="All">
                                    <option>Failed (F)</option>
                                    <option>Poor (P)</option>
                                </FormControl>
                            </FormGroup>
                        </div>
                        <div className="row-filter">
                            <FormGroup className ="project-tag">
                                <ControlLabel>Project Tag</ControlLabel>
                                <FormControl type="text" className="project-field"/>
                            </FormGroup>
                            <FormGroup>
                                <Button bsStyle="primary" className="btn-primary">Search</Button>
                            </FormGroup>
                        </div>
                    </Form>
                    <Button bsStyle="primary" className="btn btn-add"><span class="icon-playlist-plus"></span>Performance check-in</Button>
                   <div className="performance-record">
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                            <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                <span class="icon-chevron-down"></span>
                            </div>
                            <div className="footer">
                                <div className="edit"><span className="icon-square-edit-outline"></span></div> 
                                <div className="grade">Grade <span className="grade-exceptional">Exceeds Expectations</span></div>
                                <div className="english-policy"><span>English Policy</span><span className="safety-below"></span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            </div>                                                                                         
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                            <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                <span class="icon-chevron-down"></span>
                            </div>
                            <div className="footer">
                                <div className="edit"><span className="icon-square-edit-outline"></span></div> 
                                <div className="grade">Grade <span className="grade-exceptional">Exceeds Expectations</span></div>
                                <div className="english-policy"><span>English Policy</span><span className="safety-above"></span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            </div>                                                                                         
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                            <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                <span class="icon-chevron-down"></span>
                            </div>
                            <div className="footer">
                                <div className="edit"><span className="icon-square-edit-outline"></span></div> 
                                <div className="grade">Grade <span className="grade-exceptional">Exceeds Expectations</span></div>
                                <div className="english-policy"><span>English Policy</span><span className="safety-below"></span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            </div>                                                                                         
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                            <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                <span class="icon-chevron-down"></span>
                            </div>
                            <div className="footer">
                                <div className="edit"><span className="icon-square-edit-outline"></span></div> 
                                <div className="grade">Grade <span className="grade-exceptional">Exceeds Expectations</span></div>
                                <div className="english-policy"><span>English Policy</span><span className="safety"></span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            </div>                                                                                         
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                            <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                <span className="icon-chevron-down"></span>
                            </div>
                            <div className="footer">
                                <div className="edit"><span className="icon-square-edit-outline"></span></div> 
                                <div className="grade">Grade <span className="grade-exceptional">Exceeds Expectations</span></div>
                                <div className="english-policy"><span>English Policy</span><span className="safety"></span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            </div>                                                                                         
                        </div>                 
                   </div>
                </div>
        );
    }
}


