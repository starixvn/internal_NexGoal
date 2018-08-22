import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Select from 'react-select';
import { FormGroup, ControlLabel, FormControl, Button, Checkbox, Row, Col} from 'react-bootstrap';
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
                            <ControlLabel>Grade</ControlLabel>
                            <FormControl componentClass="select" placeholder="Select Grade" className="grade-field">
                                <option>Failed (F)</option>
                                <option>Poor (P)</option>
                                <option>Unsatisfactory (U)</option>
                                <option>Unsatisfactory +(U+)</option>
                            </FormControl>
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>Comment</ControlLabel>
                            <FormControl componentClass="textarea" className="comment-field"/>
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>Project Tag</ControlLabel>
                            <FormControl type="text" className="project-field"/>
                        </FormGroup>
                        <FormGroup>
                            <Button bsStyle="primary" className="btn-primary">Save as draft</Button>
                            <Button bsStyle="primary" className="btn-primary">Submit</Button>
                            <Button className="btn-cancel">Cancel</Button>
                        </FormGroup>
                    </form>
                    <Checkbox>Show Performance Record</Checkbox>
                   <div className="performance-record">
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                                <div className="name">Raymond</div>
                                <span className="role">Developer</span>
                            </div>
                            <div className="head">
                                <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                                <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            </div>
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                            </div>
                            <div className="grade">Grade <span className="grade-detail">Exceeds Expectations</span></div>
                            <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            <div className="edit"><span className="fa fa-pencil-square-o"></span></div>                                                              
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                            </div>
                            <div className="head">
                                <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                                <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            </div>
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                            </div>
                            <div className="grade">Grade: <span className="grade-detail">Exceeds Expectations</span></div>
                            <div className="project">Project: <span className="project-detail">Nexgoal, Alpha</span></div>
                            <div className="edit"><span className="fa fa-pencil-square-o"></span></div>                              
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                            </div>
                            <div className="head">
                                <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                                <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            </div>
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                            </div>
                            <div className="grade">Grade <span className="grade-detail">Exceeds Expectations</span></div>
                            <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            <div className="edit"><span className="fa fa-pencil-square-o"></span></div>                              
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                            </div>
                                <div className="head">
                                    <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                                    <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                                </div>
                                <div className="comment">                                   
                                    Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                </div>
                                <div className="grade">Grade <span className="grade-detail">Exceeds Expectations</span></div>
                                <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                                <div className="edit"><span className="fa fa-pencil-square-o"></span></div>                                                                
                        </div>
                        <div className="record">
                            <div className="avatar-user">
                                <img src={avataruser} className="avatar"/>
                            </div>
                            <div className="head">
                                <div className="nominated">Nominated <span className="employee">Raymond</span>for Employee Of The Quarter 2018</div>
                                <div className="date"><div className="final-date">21 Jul 2018</div><div className="edit-date">Edited on 23 Jul 2018</div></div>
                            </div>
                            <div className="comment">                                   
                                Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                
                            </div>
                            <div className="grade">Grade <span className="grade-detail">Exceeds Expectations</span></div>
                            <div className="project">Project <span className="project-detail">Nexgoal, Alpha</span></div>
                            <div className="edit"><span className="fa fa-pencil-square-o"></span></div>                             
                        </div>                    
                   </div>
                </div>
        );
    }
}


